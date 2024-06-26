using System.Runtime.InteropServices;

namespace ClubOrganizer.Lua;

public static class LuaCAuxTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct luaL_Reg
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string name;
        public LuaCTypes.lua_CFunction func;
    }
}