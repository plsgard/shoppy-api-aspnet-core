using System;
using Shoppy.Application.Commons;
using Shoppy.Core.Auditing;

namespace Shoppy.Application.Lists.Dtos
{
    /// <summary>
    /// An existing shopping list.
    /// </summary>
    public class ListDto : EntityDto<Guid>, ICreationTime, IModificationTime
    {
        /// <summary>
        /// Name of the list.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// When the list has been created.
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// When the list has been updated, if it was.
        /// </summary>
        public DateTimeOffset? ModificationTime { get; set; }
    }
}
