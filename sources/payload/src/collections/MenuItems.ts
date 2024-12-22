import type { CollectionConfig } from 'payload'

export const MenuItems: CollectionConfig = {
  slug: 'menu-items',
  admin: {
    useAsTitle: 'name',
    defaultColumns: ['name', 'restaurant', 'price', 'size'],
  },
  access: {
    read: () => true,
  },
  fields: [
    {
      name: 'name',
      type: 'text',
      required: true,
      admin: {
        placeholder: 'Spaghetti Bolognese',
      },
    },
    {
      name: 'restaurant',
      type: 'relationship',
      relationTo: 'restaurants',
      required: true,
      hasMany: false,
    },
    {
      name: 'price',
      type: 'number',
      required: true,
      min: 0,
      admin: {
        placeholder: '15.99',
        description: 'Price in EUR',
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
    },
  ],
}
