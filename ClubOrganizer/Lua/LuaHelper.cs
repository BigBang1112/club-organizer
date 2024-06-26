using static ClubOrganizer.Lua.LuaCTypes;

namespace ClubOrganizer.Lua;

public static class LuaHelper
{
    public static void RegisterFieldFunction(lua_State state, lua_CFunction func, string name)
    {
        LuaC.lua_pushcfunction(state, func);
        LuaC.lua_setfield(state, -2, name);
    }
}
