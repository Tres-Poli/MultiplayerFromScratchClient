﻿namespace Core
{
    public interface ITickController
    {
        IFinite AddController(IUpdateController updateController);
        IFinite AddController(IFixedController fixedController);
        IFinite AddController(ITimestampController fTimestampController);
    }
}