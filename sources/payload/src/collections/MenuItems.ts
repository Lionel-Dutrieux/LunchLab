import type { CollectionConfig } from 'payload'

export const MenuItems: CollectionConfig = {
  slug: 'menu-items',
  admin: {
    useAsTitle: 'name',
    defaultColumns: ['name', 'restaurant', 'price', 'size'],
    group: 'Food Orders',
    description: 'Manage menu items for restaurants',
  },
  access: {
    read: () => true,
  },
  fields: [
    {
      type: 'tabs',
      tabs: [
        {
          label: 'Item Details',
          fields: [
            {
              name: 'name',
              type: 'text',
              required: true,
              admin: {
                description: 'Name of the menu item',
                placeholder: 'e.g., Spaghetti Bolognese',
              },
            },
            {
              name: 'restaurant',
              type: 'relationship',
              relationTo: 'restaurants',
              required: true,
              hasMany: false,
              admin: {
                description: 'Restaurant this menu item belongs to',
              },
            },
            {
              type: 'row',
              fields: [
                {
                  name: 'price',
                  type: 'number',
                  required: true,
                  min: 0,
                  admin: {
                    placeholder: '15.99',
                    description: 'Price in EUR',
                    step: 0.01,
                    width: '50%',
                  },
                },
                {
                  name: 'size',
                  type: 'select',
                  required: true,
                  options: [
                    { label: 'Small', value: 'small' },
                    { label: 'Regular', value: 'regular' },
                    { label: 'Large', value: 'large' },
                  ],
                  defaultValue: 'regular',
                  admin: {
                    description: 'Portion size',
                    width: '50%',
                  },
                },
              ],
            },
          ],
        },
      ],
    },
  ],
}
