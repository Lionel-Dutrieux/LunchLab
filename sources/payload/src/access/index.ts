import type { Access, FieldAccess } from 'payload'
import type { User } from '../collections/Users'

export const accessControl = {
  isAdmin: (({ req }) => {
    const user = req.user as User | null
    return user?.role === 'admin'
  }) as Access,

  isAdminOrSelf: (({ req, id }) => {
    const user = req.user as User | null
    if (!user) return false
    if (user.role === 'admin') return true
    return user.id === id
  }) as Access,

  isAdminFieldLevel: (({ req }) => {
    const user = req.user as User | null
    return user?.role === 'admin'
  }) as FieldAccess,
} as const

export type { AccessControlProvider } from './types'
