﻿using AutoMapper;
using Evol.Common;
using System.Collections.Generic;

namespace Evol.TMovie.Domain.Dto
{
    /// <summary>
    /// Object-object mapping
    /// please init <see cref="DtoObjectMapInitiator"/>
    /// </summary>
    public static class IObjectMapExtensions
    {
        /// <summary>
        /// Object-object mapping
        /// </summary>
        public static TTo Map<TTo>(this object source) where TTo : new()
        {
            return Mapper.Map<TTo>(source);
        }

        /// <summary>
        /// Object-object mapping for Paged
        /// </summary>
        public static IPaged<TTo> MapPaged<TTo>(this IPaged<object> source) where TTo : new()
        {
            var tos = source.MapList<TTo>();
            var toPaged = new PagedList<TTo>(tos, source.RecordTotal, source.Index, source.Size);
            return toPaged;
        }

        /// <summary>
        /// Object-object mapping for Collection
        /// </summary>
        public static IEnumerable<TTo> MapList<TTo>(this IEnumerable<object> source) where TTo : new()
        {
            var tos = new List<TTo>();
            foreach (var item in source)
            {
                var to = item.Map<TTo>();
                tos.Add(to);
            }
            return tos;
        }
    }
}
