﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Utils;
using Xunit;

namespace Avalonia.Controls.TreeDataGridTests
{
    public class TreeSelectionModelBaseTests_Single
    {
        public class SelectedIndex
        {
            [Fact]
            public void Can_Set_SelectedIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Empty(e.DeselectedItems);
                    Assert.Equal(new IndexPath(0, 2), e.SelectedIndexes.Single());
                    Assert.Equal("Node 0-2", e.SelectedItems.Single().Caption);
                    ++raised;
                };

                target.SelectedIndex = new IndexPath(0, 2);

                Assert.Equal(1, raised);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndex);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndexes.Single());
                Assert.Equal("Node 0-2", target.SelectedItem!.Caption);
                Assert.Equal("Node 0-2", target.SelectedItems.Single()!.Caption);
            }

            [Fact]
            public void Setting_SelectedIndex_Clears_Old_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 1);
                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new IndexPath(0, 1), e.DeselectedIndexes.Single());
                    Assert.Equal("Node 0-1", e.DeselectedItems.Single().Caption);
                    Assert.Equal(new IndexPath(0, 2), e.SelectedIndexes.Single());
                    Assert.Equal("Node 0-2", e.SelectedItems.Single().Caption);
                    ++raised;
                };

                target.SelectedIndex = new IndexPath(0, 2);

                Assert.Equal(1, raised);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndex);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndexes.Single());
                Assert.Equal("Node 0-2", target.SelectedItem!.Caption);
                Assert.Equal("Node 0-2", target.SelectedItems.Single()!.Caption);
            }

            [Fact]
            public void Can_Set_SelectedIndex_To_Empty()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 2);
                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new IndexPath(0, 2), e.DeselectedIndexes.Single());
                    Assert.Equal("Node 0-2", e.DeselectedItems.Single().Caption);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.SelectedIndex = default;

                Assert.Equal(1, raised);
                Assert.Equal(default, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
            }

            [Fact]
            public void Out_Of_Range_SelectedIndex_Clears_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 2);
                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new IndexPath(0, 2), e.DeselectedIndexes.Single());
                    Assert.Equal("Node 0-2", e.DeselectedItems.Single().Caption);
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.SelectedIndex = new IndexPath(5, 10, 250);

                Assert.Equal(1, raised);
                Assert.Equal(default, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
            }

            [Fact]
            public void Can_Select_Unexpanded_Item()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Empty(e.DeselectedItems);
                    Assert.Equal(new IndexPath(1, 2), e.SelectedIndexes.Single());
                    Assert.Equal("Node 1-2", e.SelectedItems.Single().Caption);
                    ++raised;
                };

                target.SelectedIndex = new IndexPath(1, 2);

                Assert.Equal(1, raised);
                Assert.Equal(new IndexPath(1, 2), target.SelectedIndex);
                Assert.Equal(new IndexPath(1, 2), target.SelectedIndexes.Single());
                Assert.Equal("Node 1-2", target.SelectedItem!.Caption);
                Assert.Equal("Node 1-2", target.SelectedItems.Single()!.Caption);
            }

            [Fact]
            public void Setting_SelectedIndex_During_CollectionChanged_Results_In_Correct_Selection()
            {
                var data = new AvaloniaList<Node>();
                var target = CreateTarget(data);
                var binding = new MockBinding(target, data);

                data.Add(new Node());

                Assert.Equal(new IndexPath(0), target.SelectedIndex);
            }

            [Fact]
            public void PropertyChanged_Is_Raised()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = new IndexPath(0, 2);

                Assert.Equal(1, raised);
            }

            private class MockBinding : ICollectionChangedListener
            {
                private readonly TestTreeSelectionModel _target;

                public MockBinding(TestTreeSelectionModel target, AvaloniaList<Node> data)
                {
                    _target = target;
                    CollectionChangedEventManager.Instance.AddListener(data, this);
                }

                public void Changed(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
                {
                    _target.Select(new IndexPath(0));
                }

                public void PostChanged(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
                {
                }

                public void PreChanged(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
                {
                }
            }
        }

        public class SelectedItem
        {
            [Fact]
            public void PropertyChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedItem))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = new IndexPath(1);

                Assert.Equal(1, raised);
            }
        }

        public class SelectedIndexes
        {
            [Fact]
            public void PropertyChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedIndexes))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = new IndexPath(1);

                Assert.Equal(1, raised);
            }
        }

        public class SelectedItems
        {
            [Fact]
            public void PropertyChanged_Is_Raised_When_SelectedIndex_Changes()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SelectedItems))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = new IndexPath(1);

                Assert.Equal(1, raised);
            }
        }
        
        public class Select
        {
            [Fact]
            public void Select_Sets_SelectedIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Empty(e.DeselectedIndexes);
                    Assert.Empty(e.DeselectedItems);
                    Assert.Equal(new IndexPath(0, 2), e.SelectedIndexes.Single());
                    Assert.Equal("Node 0-2", e.SelectedItems.Single().Caption);
                    ++raised;
                };

                target.Select(new IndexPath(0, 2));

                Assert.Equal(1, raised);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndex);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndexes.Single());
                Assert.Equal("Node 0-2", target.SelectedItem!.Caption);
                Assert.Equal("Node 0-2", target.SelectedItems.Single()!.Caption);
            }

            [Fact]
            public void Select_Clears_Old_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.Select(new IndexPath(0, 1));

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new IndexPath(0, 1), e.DeselectedIndexes.Single());
                    Assert.Equal("Node 0-1", e.DeselectedItems.Single().Caption);
                    Assert.Equal(new IndexPath(0, 2), e.SelectedIndexes.Single());
                    Assert.Equal("Node 0-2", e.SelectedItems.Single().Caption);
                    ++raised;
                };

                target.Select(new IndexPath(0, 2));

                Assert.Equal(1, raised);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndex);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndexes.Single());
                Assert.Equal("Node 0-2", target.SelectedItem!.Caption);
                Assert.Equal("Node 0-2", target.SelectedItems.Single()!.Caption);
            }

            [Fact]
            public void Select_With_Invalid_Index_Does_Nothing()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 2);
                target.SelectionChanged += (s, e) => ++raised;

                target.Select(new IndexPath(5, 10, 250));

                Assert.Equal(0, raised);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndex);
                Assert.Equal(new IndexPath(0, 2), target.SelectedIndexes.Single());
                Assert.Equal("Node 0-2", target.SelectedItem!.Caption);
                Assert.Equal("Node 0-2", target.SelectedItems.Single()!.Caption);
            }

            [Fact]
            public void Selecting_Already_Selected_Item_Doesnt_Raise_SelectionChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 2);
                target.SelectionChanged += (s, e) => ++raised;

                target.Select(new IndexPath(0, 2));

                Assert.Equal(0, raised);
            }
        }

        public class Deselect
        {
            [Fact]
            public void Deselect_Clears_Current_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 1);

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { new IndexPath(0, 1) }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "Node 0-1" }, e.DeselectedItems.Select(x => x.Caption));
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.Deselect(new IndexPath(0, 1));

                Assert.Equal(default, target.SelectedIndex);
                Assert.Empty(target.SelectedIndexes);
                Assert.Null(target.SelectedItem);
                Assert.Empty(target.SelectedItems);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Deselect_Does_Nothing_For_Nonselected_Item()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 1);
                target.SelectionChanged += (s, e) => ++raised;
                target.Deselect(new IndexPath(0, 0));

                Assert.Equal(new IndexPath(0, 1), target.SelectedIndex);
                Assert.Equal(new[] { new IndexPath(0, 1) }, target.SelectedIndexes);
                Assert.Equal("Node 0-1", target.SelectedItem!.Caption);
                Assert.Equal(new[] { "Node 0-1" }, target.SelectedItems.Select(x => x.Caption));
                Assert.Equal(0, raised);
            }
        }

        public class Clear
        {
            [Fact]
            public void Clear_Raises_SelectionChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.Select(new IndexPath(0, 1));

                target.SelectionChanged += (s, e) =>
                {
                    Assert.Equal(new[] { new IndexPath(0, 1) }, e.DeselectedIndexes);
                    Assert.Equal(new[] { "Node 0-1" }, e.DeselectedItems.Select(x => x.Caption));
                    Assert.Empty(e.SelectedIndexes);
                    Assert.Empty(e.SelectedItems);
                    ++raised;
                };

                target.Clear();

                Assert.Equal(1, raised);
            }
        }

        public class AnchorIndex
        {
            [Fact]
            public void Setting_SelectedIndex_Sets_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = new IndexPath(0, 1);

                Assert.Equal(new IndexPath(0, 1), target.AnchorIndex);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Setting_SelectedIndex_To_Empty_Doesnt_Clear_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 1);

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = default;

                Assert.Equal(new IndexPath(0, 1), target.AnchorIndex);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Select_Sets_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.Select(new IndexPath(0, 1));

                Assert.Equal(new IndexPath(0, 1), target.AnchorIndex);
                Assert.Equal(1, raised);
            }

            [Fact]
            public void Deselect_Doesnt_Clear_AnchorIndex()
            {
                var target = CreateTarget();
                var raised = 0;

                target.Select(new IndexPath(0, 1));

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.Deselect(new IndexPath(0, 1));

                Assert.Equal(new IndexPath(0, 1), target.AnchorIndex);
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Raises_PropertyChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.AnchorIndex))
                    {
                        ++raised;
                    }
                };

                target.SelectedIndex = new IndexPath(0, 1);

                Assert.Equal(1, raised);
            }
        }

        public class SingleSelect
        {
            [Fact]
            public void Converting_To_Multiple_Selection_Preserves_Selection()
            {
                var target = CreateTarget();
                var raised = 0;

                target.SelectedIndex = new IndexPath(0, 1);

                target.SelectionChanged += (s, e) => ++raised;

                target.SingleSelect = false;

                Assert.Equal(new IndexPath(0, 1), target.SelectedIndex);
                Assert.Equal(new[] { new IndexPath(0, 1) }, target.SelectedIndexes);
                Assert.Equal("Node 0-1", target.SelectedItem!.Caption);
                Assert.Equal(new[] { "Node 0-1" }, target.SelectedItems.Select(x => x.Caption));
                Assert.Equal(0, raised);
            }

            [Fact]
            public void Raises_PropertyChanged()
            {
                var target = CreateTarget();
                var raised = 0;

                target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(target.SingleSelect))
                    {
                        ++raised;
                    }
                };

                target.SingleSelect = false;

                Assert.Equal(1, raised);
            }
        }

        private static AvaloniaList<Node> CreateNodes(IndexPath parentId)
        {
            var result = new AvaloniaList<Node>();

            for (var i = 0; i < 5; ++i)
            {
                var id = parentId.CloneWithChildIndex(i);

                var node = new Node
                {
                    Id = id,
                    Caption = "Node " + string.Join("-", id.ToArray()),
                };

                result.Add(node);
            }

            return result;
        }

        private static AvaloniaList<Node> CreateData()
        {
            return CreateNodes(default);
        }

        private static TestTreeSelectionModel CreateTarget(AvaloniaList<Node>? data = null)
        {
            return new TestTreeSelectionModel(data ?? CreateData());
        }

        private class Node
        {
            public IndexPath Id { get; set; }
            public string? Caption { get; set; }
            public AvaloniaList<Node>? Children { get; set; }
        }

        private class TestTreeSelectionModel : TreeSelectionModelBase<Node>
        {
            public TestTreeSelectionModel(AvaloniaList<Node> data)
                : base(data)
            {
            }

            protected internal override IEnumerable<Node>? GetChildren(Node node)
            {
                if (node.Children is null && node.Id.GetSize() < 2)
                    node.Children = CreateNodes(node.Id);
                return node.Children;
            }
        }
    }
}
