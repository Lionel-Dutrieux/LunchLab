import type { CollectionConfig } from 'payload'
import { accessControl } from '../access'

type PollOption = {
  votes?: { user: string; votedAt: string }[]
}

export const Polls: CollectionConfig = {
  slug: 'polls',
  admin: {
    useAsTitle: 'title',
  },
  access: {
    read: () => true,
    create: accessControl.isLoggedIn,
    update: accessControl.isLoggedIn,
    delete: accessControl.isAdminOrSelf,
  },
  fields: [
    {
      name: 'title',
      type: 'text',
      required: true,
    },
    {
      name: 'status',
      type: 'select',
      required: true,
      defaultValue: 'active',
      options: [
        { label: 'Active', value: 'active' },
        { label: 'Closed', value: 'closed' },
      ],
    },
    {
      name: 'createdBy',
      type: 'relationship',
      relationTo: 'users',
      admin: {
        position: 'sidebar',
        readOnly: true,
      },
    },
    {
      name: 'totalVotes',
      type: 'number',
      admin: {
        position: 'sidebar',
        readOnly: true,
      },
    },
    {
      name: 'options',
      type: 'array',
      required: true,
      minRows: 1,
      validate: (options) => {
        if (!Array.isArray(options)) return true

        const restaurantIds = options.map((option) => (option as { restaurant: string }).restaurant)
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
        },
        {
          name: 'votes',
          type: 'array',
          admin: {
            description: 'Users who voted for this option',
          },
          validate: (votes, { req }) => {
            if (!Array.isArray(votes)) return true

            const userVotes = (votes as Array<{ user: string }>).filter(
              (vote) => vote.user === req.user?.id,
            )
            if (userVotes.length > 1) {
              return 'Users can only vote once per poll'
            }
            return true
          },
          fields: [
            {
              name: 'user',
              type: 'relationship',
              relationTo: 'users',
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
          },
        },
      ],
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
      },
    },
    {
      name: 'mostVoted',
      type: 'relationship',
      relationTo: 'restaurants',
      admin: {
        position: 'sidebar',
        readOnly: true,
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
