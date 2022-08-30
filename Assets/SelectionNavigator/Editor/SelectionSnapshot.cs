using System;
using Object = UnityEngine.Object;

namespace SpaceCan.SelectionNavigator
{
    [Serializable]
    public class SelectionSnapshot
    {
        public Object ActiveObject;
        public Object[] Objects;
        public Object Context;

        public bool IsEmpty => this.ActiveObject == null;

        public SelectionSnapshot(Object activeObject, Object[] objects, Object context = null)
        {
            this.ActiveObject = activeObject;
            this.Objects = objects;
            this.Context = context;
        }

        public override string ToString()
        {
            return $"Active: {this.ActiveObject} Objects: {this.Objects.Length} Context: {this.Context}";
        }

        public static bool operator ==(SelectionSnapshot a, SelectionSnapshot b)
        {
            if (a is null || b is null) return false;
            bool status = false;
            if (a.ActiveObject == b.ActiveObject) status = true;
            return status;
        }

        public static bool operator !=(SelectionSnapshot a, SelectionSnapshot b)
        {
            if (a is null || b is null) return true;
            bool status = false;
            if (a.ActiveObject != b.ActiveObject || a.Objects == b.Objects) status = true;
            return status;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != typeof(SelectionSnapshot)) return false;
            return this == (SelectionSnapshot)obj;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}