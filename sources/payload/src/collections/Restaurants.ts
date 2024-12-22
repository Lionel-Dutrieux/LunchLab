import type { CollectionConfig } from 'payload'

export const Restaurants: CollectionConfig = {
  slug: 'restaurants',
  admin: {
    useAsTitle: 'name',
  },
  access: {
    read: () => true,
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
}
