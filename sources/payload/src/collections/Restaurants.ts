import type { CollectionConfig } from 'payload'

export const Restaurants: CollectionConfig = {
  slug: 'restaurants',
  admin: {
    useAsTitle: 'name',
  },
  access: {
    read: () => true, // Anyone can read restaurants
  },
  fields: [
    {
      name: 'name',
      type: 'text',
      required: true,
    },
    {
      name: 'description',
      type: 'textarea',
      required: true,
    },
    {
      name: 'address',
      type: 'text',
      required: true,
      admin: {
        placeholder: 'Rue de la Loi 16, 1000 Brussels',
      },
    },
    {
      name: 'image',
      type: 'upload',
      relationTo: 'media',
      required: true,
    },
    {
      name: 'menu',
      type: 'array',
      label: 'Menu Items',
      admin: {
        description: 'Add meals to the restaurant menu',
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
    },
  ],
}
