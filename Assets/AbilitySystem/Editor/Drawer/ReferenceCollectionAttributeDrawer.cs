using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    class ReferenceCollectionAttributeDrawer : OdinAttributeDrawer<ReferenceCollectionAttribute>
    {
        private Action m_AddItem;

        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ChildResolver is ICollectionResolver;
        }

        protected override void Initialize()
        {
            base.Initialize();
            ICollectionResolver collectionResolver = Property.ChildResolver.As<ICollectionResolver>();
            Type                elementType        = collectionResolver.ElementType;
            m_AddItem = AddItem;

            void AddItem()
            {
                elementType.ShowImplementSelector(SelectionConfirmed);
            }

            void SelectionConfirmed(IEnumerable<Type> selection)
            {
                collectionResolver.QueueAdd(Activator.CreateInstance(selection.First()).TempArray());
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            CollectionDrawerStaticInfo.NextCustomAddFunction = m_AddItem;
            CallNextDrawer(label);
        }
    }
}