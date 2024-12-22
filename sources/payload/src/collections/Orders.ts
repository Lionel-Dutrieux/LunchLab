import type { CollectionConfig } from 'payload'

export const Orders: CollectionConfig = {
  slug: 'orders',
  admin: {
    useAsTitle: 'restaurant',
    defaultColumns: ['restaurant', 'poll', 'createdBy', 'status', 'createdAt'],
  },
  access: {
    read: () => true,
    create: () => true, // Anyone can create
    update: () => true, // Anyone can update
    delete: () => true, // Anyone can delete their orders
  },
  fields: [
    {
      name: 'restaurant',
      type: 'relationship',
      relationTo: 'restaurants',
      required: true,
    },
    {
      name: 'poll',
      type: 'relationship',
      relationTo: 'polls',
      admin: {
        description: 'Link this order to a poll (optional)',
      },
    },
    {
      name: 'status',
      type: 'select',
      required: true,
      defaultValue: 'pending',
      options: [
        { label: 'Pending', value: 'pending' },
        { label: 'Confirmed', value: 'confirmed' },
        { label: 'Cancelled', value: 'cancelled' },
      ],
    },
    {
      name: 'items',
      type: 'array',
      required: true,
      minRows: 1,
      fields: [
        {
          name: 'type',
          type: 'select',
          required: true,
          defaultValue: 'menu',
          options: [
            { label: 'Menu Item', value: 'menu' },
            { label: 'Custom', value: 'custom' },
          ],
        },
        {
          name: 'menuItem',
          type: 'relationship',
          relationTo: 'menu-items',
          required: true,
          hasMany: false,
          filterOptions: ({ siblingData, data }) => {
            // Get the restaurant ID from the parent order document
            const restaurantId = data?.restaurant

            if (!restaurantId) return false // If no restaurant selected, show no options

            return {
              restaurant: {
                equals: restaurantId,
              },
            }
          },
          admin: {
            condition: (data, siblingData) => siblingData?.type === 'menu',
            description: 'Select a menu item from the chosen restaurant',
          },
        },
        {
          name: 'customItem',
          type: 'text',
          admin: {
            condition: (data, siblingData) => siblingData?.type === 'custom',
            description: 'Enter your custom order item',
          },
        },
        {
          name: 'quantity',
          type: 'number',
          required: true,
          min: 1,
          defaultValue: 1,
        },
        {
          name: 'notes',
          type: 'textarea',
          admin: {
            description: 'Any special requests or notes for this item',
          },
        },
        {
          name: 'orderedBy',
          type: 'relationship',
          relationTo: 'users',
          required: true,
          defaultValue: ({ req }) => req.user?.id,
        },
      ],
    },
    {
      name: 'totalAmount',
      type: 'number',
      admin: {
        position: 'sidebar',
        readOnly: true,
      },
    },
    {
      name: 'createdBy',
      type: 'relationship',
      relationTo: 'users',
      required: true,
      defaultValue: ({ req }) => req.user?.id,
      admin: {
        position: 'sidebar',
        readOnly: true,
      },
    },
  ],
  hooks: {
    beforeChange: [
      ({ req, data }) => {
        // Set creator if not set
        if (!data.createdBy && req.user) {
          data.createdBy = req.user.id
        }

        return data
      },
    ],
  },
}
