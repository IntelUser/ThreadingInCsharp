using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TestWW3
{
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }


    public static class CanvasExtensions
    {
        /// <summary>
        /// Removes all instances of a type of object from the children collection.
        /// </summary>
        /// <typeparam name="T">The type of object you want to remove.</typeparam>
        /// <param name="targetCollection">A reference to the canvas you want items removed from.</param>
        public static void Remove<T>(this UIElementCollection targetCollection)
        {
            // This will loop to the end of the children collection.
            int index = 0;

            // Loop over every element in the children collection.
            while (index < targetCollection.Count)
            {
                // Remove the item if it's of type T
                if (targetCollection[index] is T)
                    targetCollection.RemoveAt(index);
                else
                    index++;
            }
        }
    }
}
