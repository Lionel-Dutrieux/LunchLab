import type { CollectionConfig } from 'payload'

// Add interface at the top of file
interface OrderItem {
  quantity?: number
  paymentStatus?: string
  paidAt?: string
}

export const Orders: CollectionConfig = {
  slug: 'orders',
  admin: {
    useAsTitle: 'displayTitle',
    defaultColumns: ['displayTitle', 'status', 'createdBy', 'createdAt'],
    group: 'Food Orders',
    description: 'Manage food orders from different restaurants',
  },
  access: {
    read: () => true,
    create: () => true, // Anyone can create
    update: () => true, // Anyone can update
    delete: () => true, // Anyone can delete their orders
  },
  fields: [
    {
      name: 'displayTitle',
      type: 'text',
      admin: {
        hidden: true,
      },
    },
    {
      type: 'tabs',
      tabs: [
        {
          label: 'Order Details',
          fields: [
            {
              name: 'restaurant',
              type: 'relationship',
              relationTo: 'restaurants',
              required: true,
              admin: {
                description: 'Select the restaurant for this order',
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
              admin: {
                description: 'Current status of the order',
                style: {
                  width: '50%',
                },
              },
            },
            {
              name: 'poll',
              type: 'relationship',
              relationTo: 'polls',
              admin: {
                description: 'Link this order to a group poll (optional)',
                style: {
                  width: '50%',
                },
              },
            },
          ],
        },
        {
          label: 'Order Items',
          fields: [
            {
              name: 'items',
              type: 'array',
              required: true,
              minRows: 1,
              admin: {
                description: 'Add items to this order',
              },
              fields: [
                {
                  type: 'row',
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
                      admin: {
                        width: '50%',
                      },
                    },
                    {
                      name: 'quantity',
                      type: 'number',
                      required: true,
                      min: 1,
                      defaultValue: 1,
                      admin: {
                        width: '50%',
                        description: 'Number of items',
                      },
                    },
                  ],
                },
                {
                  name: 'menuItem',
                  type: 'relationship',
                  relationTo: 'menu-items',
                  required: true,
                  hasMany: false,
                  filterOptions: ({ siblingData, data }) => {
                    const restaurantId = data?.restaurant
                    if (!restaurantId) return false
                    return {
                      restaurant: {
                        equals: restaurantId,
                      },
                    }
                  },
                  admin: {
                    condition: (data, siblingData) => siblingData?.type === 'menu',
                    description: 'Select a menu item from the restaurant',
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
                  name: 'notes',
                  type: 'textarea',
                  admin: {
                    description: 'Special requests or notes',
                  },
                },
                {
                  type: 'row',
                  fields: [
                    {
                      name: 'orderedBy',
                      type: 'relationship',
                      relationTo: 'users',
                      required: true,
                      defaultValue: ({ req }) => req.user?.id,
                      admin: {
                        width: '50%',
                        description: 'Person who ordered this item',
                      },
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
                      admin: {
                        width: '50%',
                        description: 'Payment status for this item',
                      },
                    },
                  ],
                },
              ],
            },
          ],
        },
      ],
    },
    {
      name: 'summary',
      type: 'group',
      admin: {
        position: 'sidebar',
        description: 'Order Summary',
      },
      fields: [
        {
          name: 'itemCount',
          type: 'number',
          admin: {
            readOnly: true,
            description: 'Total number of items',
          },
        },
        {
          name: 'paidItemsCount',
          type: 'number',
          admin: {
            readOnly: true,
            description: 'Number of paid items',
          },
        },
      ],
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
        description: 'Order created by',
      },
    },
  ],
  hooks: {
    beforeChange: [
      ({ req, data, operation }) => {
        // Set creator on create
        if (operation === 'create' && req.user) {
          data.createdBy = req.user.id
        }

        // Calculate summary fields
        if (data.items?.length) {
          data.summary = {
            itemCount: data.items.length,
            paidItemsCount: data.items.filter((item: OrderItem) => item.paymentStatus === 'paid')
              .length,
          }
        }

        // Update payment timestamps
        if (data.items) {
          data.items = data.items.map((item: OrderItem) => {
            if (item.paymentStatus === 'paid' && !item.paidAt && req.user) {
              return {
                ...item,
                paidAt: new Date().toISOString(),
                markedAsPaidBy: req.user.id,
              }
            }
            return item
          })
        }

        // Add order title generation
        const timestamp = new Date().toLocaleString('en-GB', {
          day: 'numeric',
          month: 'short',
          hour: '2-digit',
          minute: '2-digit',
        })
        data.orderTitle = `Order - ${timestamp} (${data.status || 'pending'})`

        return data
      },
    ],
    afterRead: [
      ({ doc }) => {
        // Format the date using European format
        const timestamp = new Date(doc.createdAt).toLocaleString('en-GB', {
          day: 'numeric',
          month: 'short',
          hour: '2-digit',
          minute: '2-digit',
        })

        // Get restaurant name, handling both populated and unpopulated cases
        const restaurantName =
          typeof doc.restaurant === 'object' && doc.restaurant ? doc.restaurant.name : 'Order'

        // Set the display title
        doc.displayTitle = `${restaurantName} - ${timestamp}`

        return doc
      },
    ],
  },
}
