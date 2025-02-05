﻿using System.Collections.Generic;

namespace Nuve.Morphologic
{
    internal interface IGraph<T>
    {
        bool ContainsEdge(T source, T target);
        bool TryGetEdge(T source, T target, out Transition<T> edge);
        bool TryGetOutEdges(T source, out IEnumerable<Transition<T>> outEdges);
        void AddEdge(Transition<T> edge);
        void AddEdges(IEnumerable<Transition<T>> edges);
    }
}