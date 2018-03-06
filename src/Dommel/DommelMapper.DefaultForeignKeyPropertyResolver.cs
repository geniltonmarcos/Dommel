﻿using System;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Implements the <see cref="IForeignKeyPropertyResolver"/> interface.
        /// </summary>
        public class DefaultForeignKeyPropertyResolver : IForeignKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the foreign key property for the specified source type and including type
            /// by using <paramref name="includingType"/> + Id as property name.
            /// </summary>
            /// <param name="sourceType">The source type which should contain the foreign key property.</param>
            /// <param name="includingType">The type of the foreign key relation.</param>
            /// <param name="foreignKeyRelation">The foreign key relationship type.</param>
            /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
            public virtual PropertyInfo ResolveForeignKeyProperty(Type sourceType, Type includingType, out ForeignKeyRelation foreignKeyRelation)
            {
                var oneToOneFk = ResolveOneToOne(sourceType, includingType);
                if (oneToOneFk != null)
                {
                    foreignKeyRelation = ForeignKeyRelation.OneToOne;
                    return oneToOneFk;
                }

                var oneToManyFk = ResolveOneToMany(sourceType, includingType);
                if (oneToManyFk != null)
                {
                    foreignKeyRelation = ForeignKeyRelation.OneToMany;
                    return oneToManyFk;
                }

                var msg = $"Could not resolve foreign key property. Source type '{sourceType.FullName}'; including type: '{includingType.FullName}'.";
                throw new Exception(msg);
            }

            private static PropertyInfo ResolveOneToOne(Type sourceType, Type includingType)
            {
                // Look for the foreign key on the source type.
                var foreignKeyName = includingType.Name + "Id";
                var foreignKeyProperty = sourceType.GetProperties().FirstOrDefault(p => p.Name == foreignKeyName);

                return foreignKeyProperty;
            }

            private static PropertyInfo ResolveOneToMany(Type sourceType, Type includingType)
            {
                // Look for the foreign key on the including type.
                var foreignKeyName = sourceType.Name + "Id";
                var foreignKeyProperty = includingType.GetProperties().FirstOrDefault(p => p.Name == foreignKeyName);
                return foreignKeyProperty;
            }
        }
    }
}
