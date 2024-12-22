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
        {
          name: 'paymentStatus',
          type: 'select',
          required: true,
          defaultValue: 'pending',
          options: [
            { label: 'Pending Payment', value: 'pending' },
            { label: 'Paid', value: 'paid' },
          ],
          access: {
            update: ({ req, data, siblingData }) => {
              if (!req.user) return false
              // Allow update if user is either the order creator or the person who ordered this item
              return req.user.id === data?.createdBy || req.user.id === siblingData?.orderedBy
            },
          },
        },
        {
          name: 'paidAt',
          type: 'date',
          admin: {
            position: 'sidebar',
            readOnly: true,
            description: 'Automatically set when payment status changes to paid',
          },
        },
        {
          name: 'markedAsPaidBy',
          type: 'relationship',
          relationTo: 'users',
          admin: {
            position: 'sidebar',
            readOnly: true,
            description: 'User who marked this item as paid',
          },
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

        // Update paidAt and markedAsPaidBy when payment status changes to paid
        if (data.items) {
          data.items = data.items.map((item: any) => {
            if (item.paymentStatus === 'paid' && !item.paidAt && req.user) {
              item.paidAt = new Date().toISOString()
              item.markedAsPaidBy = req.user.id
            }
            return item
          })
        }

        return data
      },
    ],
  },
}
