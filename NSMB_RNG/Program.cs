using NSMB_RNG;

uint LCRNG_NSMB(uint v)
{
    ulong a = ((ulong)0x0019660D * v + 0x3C6EF35F);
    return (uint)(a + (a >> 32));
}

int main()
{
    TilesFor12 tf12 = new TilesFor12();
    tf12.calculatePossibleSeeds();

    return 0;
}

main();