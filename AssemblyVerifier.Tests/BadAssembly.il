.assembly extern mscorlib
{
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}

.assembly BadAssembly
{
    .hash algorithm 0x00008004
    .ver 1:0:0:0
}

.module BadAssembly.dll

.class public beforefieldinit BadClass
    extends [mscorlib]System.Object
{
    .method public hidebysig instance int32
        ReturnArg(int32 Arg) cil managed
    {
        .maxstack 1
        // Note that we're returning the "this" value :(.
        ldloc.0
        ret
    }
}