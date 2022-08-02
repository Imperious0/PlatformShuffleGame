using System;

public interface IPrize
{
    event EventHandler<PrizeCollectedArgs> PrizeCollectedEvent;
}
