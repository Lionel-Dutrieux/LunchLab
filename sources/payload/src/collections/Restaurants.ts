import type { CollectionConfig } from 'payload'

export const Restaurants: CollectionConfig = {
  slug: 'restaurants',
  admin: {
    useAsTitle: 'name',
    defaultColumns: ['name', 'address', 'menuItems', 'createdAt'],
    group: 'Food Orders',
    description: 'Manage restaurants and their menus',
  },
  access: {
    read: () => true,
  },
  fields: [
    {
      type: 'tabs',
      tabs: [
        {
          label: 'Basic Info',
          fields: [
            {
              name: 'name',
              type: 'text',
              required: true,
              admin: {
                description: 'Restaurant name',
                placeholder: 'e.g., Pizza Express',
              },
            },
            {
              name: 'description',
              type: 'textarea',
              required: true,
              admin: {
                description: 'Brief description of the restaurant',
                placeholder: 'Describe the restaurant, cuisine type, etc.',
              },
            },
            {
              name: 'address',
              type: 'text',
              required: true,
              admin: {
                description: 'Physical address of the restaurant',
                placeholder: 'Rue de la Loi 16, 1000 Brussels',
              },
            },
          ],
        },
        {
          label: 'Media',
          fields: [
            {
              name: 'image',
              type: 'upload',
              relationTo: 'media',
              required: true,
              admin: {
                description: 'Restaurant image or logo',
              },
            },
          ],
        },
        {
          label: 'Menu',
          fields: [
            {
              name: 'menuItems',
              type: 'join',
              hasMany: true,
              collection: 'menu-items',
              on: 'restaurant',
              admin: {
                description: 'Menu items linked to this restaurant',
              },
            },
          ],
        },
      ],
    },
  ],
}
