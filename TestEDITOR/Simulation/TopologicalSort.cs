// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSIM
{
    public class TopologicalSort<T> where T : class
    {
        public IEnumerable<T> Sort(
            IEnumerable<T> source,
            Func<T, IEnumerable<T>> dependencies,
            bool ignoreCycles)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();
            foreach (var item in source)
            {
                Visit(item, visited, sorted, dependencies, ignoreCycles);
            }
            return sorted;
        }

        private void Visit(
            T item,
            ICollection<T> visited,
            IList<T> sorted,
            Func<T, IEnumerable<T>> dependencies,
            bool ignoreCycles)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);
                foreach (var dep in dependencies(item))
                {
                    Visit(dep, visited, sorted, dependencies, ignoreCycles);
                }
                sorted.Add(item);
            }
            else if (!ignoreCycles && !sorted.Contains(item))
            {
                throw new Exception("Invalid dependency cycle.");
            }
        }
    }
}
