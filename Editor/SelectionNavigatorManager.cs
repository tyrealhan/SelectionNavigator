//TODO: Recover from domain reload

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace SpaceCan.SelectionNavigator
{
    [InitializeOnLoad]
    public static class SelectionNavigatorManager
    {
        private const Int32 VK_XBUTTON1 = 0x05;
        private const Int32 VK_XBUTTON2 = 0x06;

        private static HistoryQueue<SelectionSnapshot> history = new HistoryQueue<SelectionSnapshot>(15);

        private static bool ignoreSelectionChange = false;

        public static HistoryQueue<SelectionSnapshot> History => history;
        public static SelectionSnapshot Current => History.Current();
        public static bool SelectionIsEmpty => Selection.activeObject == null;
        public static int Size => history?.Size ?? 0;
        public static Action HistoryChanged;

        static SelectionNavigatorManager()
        {
#if UNITY_EDITOR_WIN
            EditorApplication.update += GetMouseButtonStates;
#endif
            Selection.selectionChanged += OnSelectionChanged;
            //AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            //AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            // EditorApplication.hierarchyChanged += ClearEmptyEntries;
            // EditorApplication.projectChanged += ClearEmptyEntries;
        }

        // private static void OnBeforeAssemblyReload()
        // {
        // }

        // private static void OnAfterAssemblyReload()
        // {
        //     HistoryChanged?.Invoke();
        // }

#if UNITY_EDITOR_WIN

        [DllImport("user32.dll")]
        public static extern Int16 GetAsyncKeyState(Int32 virtualKeyCode);

        private static void GetMouseButtonStates()
        {
            //Only register mouse button input if unity has focus
            if (!UnityEditorInternal.InternalEditorUtility.isApplicationActive) return;

            short backButtonState = GetAsyncKeyState(VK_XBUTTON1);
            short forwardButtonState = GetAsyncKeyState(VK_XBUTTON2);

            if ((backButtonState & 0x01) > 0) Back();
            if ((forwardButtonState & 0x01) > 0) Forward();
            //if ((state & 0x10000000) > 0) Debug.Log("Held!");
        }

#endif

        private static void OnSelectionChanged()
        {
            if (ignoreSelectionChange)
            {
                ignoreSelectionChange = false;
                return;
            }

            if (SelectionIsEmpty) return;
            var tmp = TakeSnapshot();
            if (tmp == history.Last())
            {//跟最后一次选择相同不添加记录，改为选中
                Select(history.ResetCurrent());
            }
            else
            {
                history.Push(tmp);
            }
            HistoryChanged?.Invoke();
        }

        public static SelectionSnapshot TakeSnapshot()
        {
            return new SelectionSnapshot(Selection.activeObject, Selection.objects, Selection.activeContext);
        }


        public static void Select(int index)
        {
            history.SetCurrent(index);
            Select(history.Current());
            HistoryChanged?.Invoke();
        }

        [Shortcut("History/Back", null, KeyCode.LeftArrow, ShortcutModifiers.Control)]
        public static void Back()
        {
            if (history.Size <= 0) return;

            var prev = SelectionIsEmpty ? history.Current() : history.Previous();

            if (prev.IsEmpty)
            {
                history.Next();
                ClearEmptyEntries();
                Back();
            }

            ignoreSelectionChange = true;
            Select(prev);
            HistoryChanged?.Invoke();
        }

        [Shortcut("History/Forward", null, KeyCode.RightArrow, ShortcutModifiers.Control)]
        public static void Forward()
        {
            if (history.Size <= 0) return;

            var next = SelectionIsEmpty ? history.Current() : history.Next();

            if (next.IsEmpty)
            {
                history.Previous();
                ClearEmptyEntries();
                Forward();
            }

            ignoreSelectionChange = true;
            Select(next);
            HistoryChanged?.Invoke();
        }

        [Shortcut("History/Clear", null)]
        public static void Clear()
        {
            history.Clear();
            HistoryChanged?.Invoke();
        }

        /// <summary>
        /// Makes selection history ignore the next selection
        /// </summary>
        public static void HideSelectionFromHistory()
        {
            ignoreSelectionChange = true;
        }

        private static void Select(SelectionSnapshot snapshot)
        {
            Selection.SetActiveObjectWithContext(snapshot.ActiveObject, snapshot.Context);
            Selection.objects = snapshot.Objects;
        }

        /// <summary>
        /// Removes deleted objects from history buffer
        /// </summary>
        private static void ClearEmptyEntries()
        {
            var array = history.ToArray().ToList();
            int current = history.GetCurrentIndex();

            for (int i = array.Count - 1; i >= 0; i--)
            {
                if (array[i].IsEmpty)
                {
                    array.RemoveAt(i);
                    if (current >= i) current--;
                }
            }

            history = HistoryQueue<SelectionSnapshot>.FromArray(array.ToArray(), current, 15);
        }
    }
}
