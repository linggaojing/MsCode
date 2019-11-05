using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
namespace LuaFramework
{
    // Token: 0x02000099 RID: 153
    public class LuaClickListener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        // Token: 0x06000E3E RID: 3646 RVA: 0x0009B924 File Offset: 0x00099B24
        public static LuaClickListener Get(GameObject go)
        {
            LuaClickListener luaClickListener = go.GetComponent<LuaClickListener>();
            bool flag = luaClickListener == null;
            if (flag)
            {
                luaClickListener = go.AddComponent<LuaClickListener>();
            }
            return luaClickListener;
        }

        // Token: 0x06000E3F RID: 3647 RVA: 0x0009B950 File Offset: 0x00099B50
        public static bool Remove(GameObject go)
        {
            LuaClickListener component = go.GetComponent<LuaClickListener>();
            bool flag = component != null;
            bool result;
            if (flag)
            {
                Object.Destroy(component);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x06000E40 RID: 3648 RVA: 0x0009B984 File Offset: 0x00099B84
        public void OnPointerClick(PointerEventData eventData)
        {
            bool flag = this.onClick != null;
            if (flag)
            {
                this.onClick(base.gameObject, eventData.position.x, eventData.position.y);
            }
        }

        // Token: 0x040006FE RID: 1790
        public LuaClickListener.VoidDelegate onClick;

        // Token: 0x02000256 RID: 598
        // (Invoke) Token: 0x060015B3 RID: 5555
        public delegate void VoidDelegate(GameObject go, float x, float y);
    }
}
