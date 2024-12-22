import type { CollectionConfig } from 'payload'
import { accessControl } from '../access'

export interface User {
  id: string
  role: 'admin' | 'editor' | 'user'
  firstName: string
  lastName: string
  trigram: string
}

export const Users: CollectionConfig = {
  slug: 'users',
  auth: true,
  admin: {
    useAsTitle: 'email',
  },
  access: {
    read: accessControl.isAdminOrSelf,
    update: accessControl.isAdminOrSelf,
    delete: accessControl.isAdmin,
    create: accessControl.isAdmin,
  },
  fields: [
    {
      name: 'role',
      type: 'select',
      required: true,
      defaultValue: 'user',
      options: [
        { label: 'Admin', value: 'admin' },
        { label: 'Editor', value: 'editor' },
        { label: 'User', value: 'user' },
      ],
      access: {
        update: accessControl.isAdminFieldLevel,
      },
    },
    {
      name: 'firstName',
      type: 'text',
      required: true,
      label: 'First Name',
    },
    {
      name: 'lastName',
      type: 'text',
      required: true,
      label: 'Last Name',
    },
    {
      name: 'trigram',
      type: 'text',
      required: true,
      maxLength: 3,
      minLength: 3,
      admin: {
        placeholder: 'JDO',
      },
    },
  ],
}
