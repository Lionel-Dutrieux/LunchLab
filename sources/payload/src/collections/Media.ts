import type { CollectionConfig } from 'payload'
import { accessControl } from '../access'

export const Media: CollectionConfig = {
  slug: 'media',
  access: {
    read: () => true,
    create: accessControl.isAdmin,
    update: accessControl.isAdmin,
    delete: accessControl.isAdmin,
  },
  fields: [
    {
      name: 'alt',
      type: 'text',
      required: true,
    },
  ],
  upload: true,
}
