
public enum ActivityType { Users = 0, ProcessFlow = 1, ProcessIntialization = 2, ExecutionActivity = 3,PostAllocationCheck=4 }
public enum ActivityCode { InputVerification = 0, SeatAllotment = 1, PostAllotmentCheck = 2, DownloadReports = 3, OutputVerification = 4, Approval = 5, ExportData = 6 ,
    ResetCurrentRound = 7, InitializeCurrentAllocation = 8,
    AllotmentStarted=20,IntializeTables = 21, EligibleCandidatesPrepared = 22, SeatMatrixPrepared = 23, OnAllocationChecksPassed = 24, OnAllocationChecksFailed = 25,
    PreviousAllotmentPrepared = 26,VirtualChoicePrepared = 27, IterationCompleted = 28, SeatConversionCompleted = 29, AllotmentSummaryPrepared=30,ApplicationUpdated=31,AllocationUnloaded=32, AllotmentFailed = 33, AbortRequested = 34, Aborted = 35, AllotmentCompleted = 36}
public enum ActionStatus { Success = 10, Failed = 11, Error = 12 }
public enum ActivityStatus { Pending = 0, Failed = 1, Completed = 2 }










