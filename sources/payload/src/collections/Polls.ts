import type { CollectionConfig } from 'payload'
import { accessControl } from '../access'

type PollOption = {
  restaurant: string
  votes?: { user: string; votedAt: string }[]
}

export const Polls: CollectionConfig = {
  slug: 'polls',
  admin: {
    useAsTitle: 'title',
    defaultColumns: ['title', 'status', 'totalVotes', 'endDate'],
    group: 'Food Orders',
    description: 'Manage restaurant voting polls',
  },
  access: {
    read: () => true,
    create: accessControl.isLoggedIn,
    update: accessControl.isLoggedIn,
    delete: accessControl.isAdminOrSelf,
  },
  fields: [
    {
      type: 'tabs',
      tabs: [
        {
          label: 'Poll Details',
          fields: [
            {
              name: 'title',
              type: 'text',
              required: true,
              admin: {
                description: 'Title of the poll',
                placeholder: 'e.g., Lunch Poll for Friday',
              },
            },
            {
              type: 'row',
              fields: [
                {
                  name: 'status',
                  type: 'select',
                  required: true,
                  defaultValue: 'active',
                  options: [
                    { label: 'Active', value: 'active' },
                    { label: 'Closed', value: 'closed' },
                  ],
                  admin: {
                    description: 'Current status of the poll',
                    width: '50%',
                  },
                },
                {
                  name: 'endDate',
                  type: 'date',
                  required: true,
                  admin: {
                    description: 'When should this poll close?',
                    date: {
                      pickerAppearance: 'dayAndTime',
                    },
                    width: '50%',
                  },
                },
              ],
            },
          ],
        },
        {
          label: 'Restaurant Options',
          fields: [
            {
              name: 'options',
              type: 'array',
              required: true,
              minRows: 1,
              admin: {
                description: 'Add restaurants to vote on',
              },
              validate: (options) => {
                if (!Array.isArray(options)) return true
                const restaurantIds = options.map(
                  (option) => (option as { restaurant: string }).restaurant,
                )
                const uniqueRestaurantIds = new Set(restaurantIds)
                if (restaurantIds.length !== uniqueRestaurantIds.size) {
                  return 'Each restaurant can only be added once to a poll'
                }
                return true
              },
              fields: [
                {
                  name: 'restaurant',
                  type: 'relationship',
                  relationTo: 'restaurants',
                  required: true,
                  admin: {
                    description: 'Select a restaurant',
                    width: '50%',
                  },
                },
                {
                  name: 'votes',
                  type: 'array',
                  admin: {
                    description: 'Users who voted for this option',
                    readOnly: false,
                    width: '50%',
                  },
                  access: {
                    create: ({ req }) => Boolean(req.user),
                    update: ({ req }) => Boolean(req.user),
                  },
                  validate: (options, { req }) => {
                    if (!Array.isArray(options)) return true
                    if (!options.length) return true
                    if (!req?.user?.id) return true

                    const latestVote = options[options.length - 1]
                    if (!latestVote) return true

                    const userId = req.user.id
                    if ((latestVote as { user: string }).user === userId) {
                      const previousVote = options
                        .slice(0, -1)
                        .find((vote) => (vote as { user: string }).user === userId)
                      if (previousVote) {
                        return 'You have already voted for this restaurant'
                      }
                    }

                    return true
                  },
                  fields: [
                    {
                      name: 'user',
                      type: 'relationship',
                      relationTo: 'users',
                      required: true,
                      defaultValue: ({ req }) => req.user?.id,
                      admin: {
                        readOnly: true,
                      },
                    },
                    {
                      name: 'votedAt',
                      type: 'date',
                      required: true,
                      defaultValue: () => new Date().toISOString(),
                      admin: {
                        readOnly: true,
                      },
                    },
                  ],
                },
                {
                  name: 'addedBy',
                  type: 'relationship',
                  relationTo: 'users',
                  defaultValue: ({ req }) => req.user?.id,
                  admin: {
                    readOnly: true,
                    description: 'User who added this option',
                  },
                },
              ],
            },
          ],
        },
      ],
    },
    {
      name: 'createdBy',
      type: 'relationship',
      relationTo: 'users',
      admin: {
        position: 'sidebar',
        readOnly: true,
        description: 'Poll created by',
      },
    },
    {
      name: 'totalVotes',
      type: 'number',
      admin: {
        position: 'sidebar',
        readOnly: true,
        description: 'Total number of votes',
      },
    },
    {
      name: 'mostVoted',
      type: 'relationship',
      relationTo: 'restaurants',
      admin: {
        position: 'sidebar',
        readOnly: true,
        description: 'Restaurant with most votes',
      },
    },
  ],
  hooks: {
    beforeChange: [
      ({ req, data, operation }) => {
        // Set creator on new polls
        if (operation === 'create' && req.user) {
          data.createdBy = req.user.id
        }

        // Auto-close polls that have ended
        if (data.endDate && new Date(data.endDate) < new Date()) {
          data.status = 'closed'
        }

        // Calculate total votes
        if (data.options) {
          data.totalVotes = data.options.reduce(
            (sum: number, option: PollOption) => sum + (option.votes?.length || 0),
            0,
          )
        }

        // Calculate most voted restaurant
        if (data.options?.length) {
          const mostVotedOption = data.options.reduce(
            (max: PollOption, curr: PollOption) =>
              (curr.votes?.length || 0) > (max.votes?.length || 0) ? curr : max,
            data.options[0],
          )
          data.mostVoted = mostVotedOption.votes?.length ? mostVotedOption.restaurant : null
        }

        return data
      },
    ],
  },
}
