import type { Access, FieldAccess } from 'payload'

export type AccessControlProvider = {
  isAdmin: Access
  isAdminOrSelf: Access
  isAdminFieldLevel: FieldAccess
}
