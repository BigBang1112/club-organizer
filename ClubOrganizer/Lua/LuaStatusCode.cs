namespace ClubOrganizer.Lua;

public enum LuaStatusCode
{
    Ok,
    Yield,
    ErrRun,
    ErrSyntax,
    ErrMem,
    ErrGcmm,
    ErrErr
}
