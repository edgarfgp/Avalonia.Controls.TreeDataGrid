﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls.Models.TreeDataGrid;

#nullable enable

namespace Avalonia.Controls.Selection
{
    internal class TreeSelectionNode<T> : SelectionNodeBase<T>
    {
        private readonly TreeSelectionModelBase<T> _owner;
        private List<TreeSelectionNode<T>?>? _children;

        public TreeSelectionNode(TreeSelectionModelBase<T> owner)
        {
            _owner = owner;
            RangesEnabled = true;
        }

        public TreeSelectionNode(
            TreeSelectionModelBase<T> owner,
            TreeSelectionNode<T> parent,
            int index)
            : this(owner)
        {
            Path = parent.Path.CloneWithChildIndex(index);
            if (parent.ItemsView is object)
                Source = _owner.GetChildren(parent.ItemsView[index]);
        }

        public IndexPath Path { get; }

        public new IEnumerable? Source
        {
            get => base.Source;
            set => base.Source = value;
        }

        public IndexPath CoerceIndex(IndexPath path, int depth)
        {
            if (path == default)
                return default;

            if (depth == path.GetSize() - 1)
            {
                var leaf = path.GetLeaf()!.Value;
                return leaf < ItemsView?.Count ? path : default;
            }

            var index = path.GetAt(depth++);
            var child = GetChild(index, false);

            if (child is object)
            {
                return child.CoerceIndex(path, depth);
            }

            var items = (IEnumerable<T>?)ItemsView;

            while (items is object)
            {
                var count = items.Count();

                if (index < count)
                {
                    items = _owner.GetChildren(items.ElementAt(index));

                    if (depth == path.GetSize() - 1)
                    {
                        return path;
                    }
                    else
                    {
                        index = path.GetAt(depth++);
                    }
                }
                else
                {
                    return default;
                }
            }

            return default;
        }

        public void Select(IndexPathRange range, TreeSelectionModelBase<T>.Operation operation)
        {
            if (ItemsView is null || ItemsView.Count == 0)
                return;

            var select = range.Intersect(Path, ItemsView.Count);

            if (select.HasValue)
            {
                CommitSelect(select.Value);
                operation.SelectedRanges ??= new();
                operation.SelectedRanges.Add(Path, select.Value);
            }
        }

        public void Deselect(IndexPathRange range, TreeSelectionModelBase<T>.Operation operation)
        {
            if (Ranges.Count > 0)
            {
                var firstSelected = Path.CloneWithChildIndex(Ranges[0].Begin);
                var lastSelected = Path.CloneWithChildIndex(Ranges[^1].End);

                if (range.FullyContains(firstSelected, lastSelected))
                {
                    var deselected = operation.DeselectedRanges ??= new();
                    foreach (var selected in Ranges)
                        deselected.Add(Path, selected);
                    CommitDeselect(new IndexRange(0, int.MaxValue));
                }

                // TODO: Intersecting ranges
            }

            if (_children is object)
            {
                foreach (var child in _children)
                    child?.Deselect(range, operation);
            }
        }

        public bool TryGetNode(
            IndexPath path,
            int depth,
            bool realize,
            [NotNullWhen(true)] out TreeSelectionNode<T>? result)
        {
            if (depth == path.GetSize())
            {
                result = this;
                return true;
            }

            var index = path.GetAt(depth);
            result = GetChild(index, realize);
            return result is object;
        }

        protected override void OnSourceCollectionChangeFinished()
        {
        }

        private protected override void OnIndexesChanged(int shiftIndex, int shiftDelta)
        {
            _owner.OnIndexesChanged(Path, shiftIndex, shiftDelta);
        }

        private protected override void OnSourceReset()
        {
            throw new NotImplementedException();
        }

        private protected override void OnSelectionChanged(IReadOnlyList<T> deselectedItems)
        {
            throw new NotImplementedException();
        }

        private TreeSelectionNode<T>? GetChild(int index, bool realize)
        {
            if (realize)
            {
                _children ??= new List<TreeSelectionNode<T>?>();

                if (ItemsView is null)
                {
                    if (_children.Count < index + 1)
                    {
                        Resize(_children, index + 1);
                    }

                    return _children[index] ??= new TreeSelectionNode<T>(_owner, this, index);
                }
                else
                {
                    if (_children.Count > ItemsView.Count)
                    {
                        throw new Exception("!!!");
                    }

                    Resize(_children, ItemsView.Count);
                    return _children[index] ??= new TreeSelectionNode<T>(_owner, this, index);
                }
            }
            else
            {
                if (_children?.Count > index)
                {
                    return _children[index];
                }
            }

            return null;
        }

        private static void Resize(List<TreeSelectionNode<T>?> list, int count)
        {
            int current = list.Count;

            if (count < current)
            {
                list.RemoveRange(count, current - count);
            }
            else if (count > current)
            {
                if (count > list.Capacity)
                {
                    list.Capacity = count;
                }

                list.InsertMany(0, null, count - current);
            }
        }
    }
}
