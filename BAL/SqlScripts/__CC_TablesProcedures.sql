insert into XT_Administrator  values ('ComCouns21','NICadmin' ,'Test@1234','12','admin')
insert into XT_Administrator  values ('ComCouns21','NICVerify', 'Test@1234','12','verify')
GO
/****** Object:  UserDefinedTableType [dbo].[XApp_CC_AllotmentSummaryTable]    Script Date: 14-08-2021 13:25:59 ******/
CREATE TYPE [dbo].[XApp_CC_AllotmentSummaryTable] AS TABLE(
	[boardId] [int] NULL,
	[roundNo] [int] NOT NULL,
	[instituteId] [varchar](6) NOT NULL,
	[programId] [varchar](7) NOT NULL,
	[streamId] [varchar](2) NOT NULL,
	[groupId] [varchar](10) NULL,
	[seatType] [varchar](10) NULL,
	[quotaId] [varchar](2) NULL,
	[categoryId] [varchar](2) NULL,
	[subcategoryId] [varchar](2) NULL,
	[genderId] [varchar](2) NULL,
	[rankTypeId] [varchar](2) NULL,
	[InItSeats] [int] NULL,
	[NewSeats] [int] NULL,
	[Allotted] [int] NULL,
	[OpeningRank] [float] NULL,
	[ClosingRank] [float] NULL,
	[OpeningRank_NewCand] [float] NULL,
	[ClosingRank_NewCand] [float] NULL,
	[DereserveFrom] [int] NULL,
	[DereserveTo] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[XApp_CC_AllotmentTable]    Script Date: 14-08-2021 13:25:59 ******/
CREATE TYPE [dbo].[XApp_CC_AllotmentTable] AS TABLE(
	[boardId] [int] NOT NULL,
	[roundNo] [int] NOT NULL,
	[rollNo] [varchar](15) NOT NULL,
	[ranktypeId] [varchar](2) NULL,
	[rank] [float] NOT NULL,
	[instituteId] [varchar](6) NOT NULL,
	[programId] [varchar](7) NOT NULL,
	[streamId] [varchar](2) NOT NULL,
	[groupId] [varchar](10) NOT NULL,
	[seatTypeId] [varchar](10) NOT NULL,
	[allottedCat] [varchar](4) NOT NULL,
	[allottedQuota] [varchar](2) NOT NULL,
	[seatGenderId] [char](1) NOT NULL
)
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_AreOutputTablesClean]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[XApp_CC_AreOutputTablesClean]
(
@boardId int,
@roundNo int,
@result int output
)
AS
BEGIN
	Declare @true int = 1, @false int= 0
	Set @result=@true;
	if exists (select top 1 '1'  from App_Allotment where boardId=@boardId and roundNo=@roundNo)
		set @result=@false
	else if exists(select top 1 '1' from App_AllotmentSummary where  boardId=@boardId and roundNo=@roundNo)
		set @result=@false
	else 
		set @result=@true
	return @result
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_CleanOutputTables]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[XApp_CC_CleanOutputTables]
(
@boardId int,
@roundNo int,
@result int output
)
AS
BEGIN 	
	Declare @Success int = 10, @Failed int= 11,@Error int =12
	Begin try
		Set @result=@Failed
		Delete from App_Allotment where boardId=@boardId and roundNo=@roundNo
		Delete from App_AllotmentSummary where  boardId=@boardId and  roundNo=@roundNo
		Set @result=@Success
	End try
	Begin catch
		Set @result=@Error
	End catch
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DereserveSeat]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Proc [dbo].[XApp_CC_DereserveSeat]
@boardId int,
@roundNo int,
@isIterationrequired varchar(10)='N' out
AS
BEGIN
	Set @isIterationrequired='N'
	if(@roundNo<99)
	return
	/*
	Declare @maxIteration int
	select @maxIteration=max(IterationNo) from XT_Dereserve
	

	If Exists (Select 1 From XT_PSeat where bSeat>0 and right(Sequence,2) in ( 'G5','G4','G3','G2'))
	Begin
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat from XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and left(S1.Sequence,4)=left(S2.Sequence,4) and right(S1.Sequence,2)='G5' and right(S2.Sequence,2)='G4' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and left(S1.Sequence,4)=left(S2.Sequence,4) and right(S1.Sequence,2)='G4' and right(S2.Sequence,2)='G3' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and left(S1.Sequence,4)=left(S2.Sequence,4) and right(S1.Sequence,2)='G3' and right(S2.Sequence,2)='G2' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and left(S1.Sequence,4)=left(S2.Sequence,4) and right(S1.Sequence,2)='G2' and right(S2.Sequence,2)='G1' and S1.bSeat>0
		
		Update D Set D.DereserveFrom=S.bSeat From XT_Dereserve D inner join XT_PSeat S on iterationNo=@maxIteration and  D.Instcd=S.Instcd and D.Brcd=S.Brcd and D.Sequence=S.Sequence and right(S.Sequence,2) in ( 'G5','G4','G3','G2')	and S.BSeat>0	
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and left(S1.Sequence,4)=left(D.Sequence,4) and right(S1.Sequence,2) ='G5' and right(D.Sequence,2)='G4' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and left(S1.Sequence,4)=left(D.Sequence,4) and right(S1.Sequence,2) ='G4' and right(D.Sequence,2)='G3' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and left(S1.Sequence,4)=left(D.Sequence,4) and right(S1.Sequence,2) ='G3' and right(D.Sequence,2)='G2' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and left(S1.Sequence,4)=left(D.Sequence,4) and right(S1.Sequence,2) ='G2' and right(D.Sequence,2)='G1' and S1.bSeat>0


		Update XT_PSeat Set tseat=tseat-bSeat,bseat=0 where right(Sequence,2) in ( 'G5','G4','G3','G2') and BSeat>0
		Set @isIterationrequired='Y'
		return
	End
	If Exists (Select 1 From XT_PSeat where bSeat>0 and Sequence in (  'BCPHG1','BCNOG1','EWPHG1','EWNOG1','SCPHG1','STPHG1','OPPHG1'))
	Begin
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat from XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and S1.Sequence='BCPHG1' and S2.Sequence='BCNOG1' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and S1.Sequence='BCNOG1' and S2.Sequence='OPNOG1' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and S1.Sequence='EWPHG1' and S2.Sequence='EWNOG1' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and S1.Sequence='EWNOG1' and S2.Sequence='OPNOG1' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and S1.Sequence='SCPHG1' and S2.Sequence='SCNOG1' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and S1.Sequence='STPHG1' and S2.Sequence='STNOG1' and S1.bSeat>0
		Update S2 Set S2.tSeat=S2.tSeat+S1.bSeat From XT_PSeat S1 inner join XT_PSeat S2 on S1.Instcd=S2.instcd and S1.brcd=S2.Brcd and S1.Sequence='OPPHG1' and S2.Sequence='OPNOG1' and S1.bSeat>0
		
		Update D Set D.DereserveFrom=S.bSeat From XT_Dereserve D inner join XT_PSeat S on iterationNo=@maxIteration and  D.Instcd=S.Instcd and D.Brcd=S.Brcd and D.Sequence=S.Sequence and S.Sequence in ('BCPHG1','BCNOG1','EWPHG1','EWNOG1','SCPHG1','STPHG1','OPPHG1')	and S.BSeat>0	
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and S1.Sequence ='BCPHG1' and D.Sequence='BCNOG1' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and S1.Sequence ='BCNOG1' and D.Sequence='OPNOG1' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and S1.Sequence ='EWPHG1' and D.Sequence='EWNOG1' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and S1.Sequence ='EWNOG1' and D.Sequence='OPNOG1' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and S1.Sequence ='SCPHG1' and D.Sequence='SCNOG1' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and S1.Sequence ='STPHG1' and D.Sequence='STNOG1' and S1.bSeat>0
		Update D Set D.DereserveTo=D.DereserveTo+S1.bSeat from XT_PSeat S1 inner join XT_Dereserve D on iterationNo=@maxIteration and S1.Instcd=D.instcd and S1.brcd=D.Brcd and S1.Sequence ='OPPHG1' and D.Sequence='OPNOG1' and S1.bSeat>0
		
		Update XT_PSeat Set tseat=tseat-bSeat,bseat=0 where Sequence in ('BCPHG1','BCNOG1','EWPHG1','EWNOG1','SCPHG1','STPHG1','OPPHG1') and BSeat>0
		Set @isIterationrequired='Y'
		return
	End
	*/
	


END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownloadAllCandidates]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[XApp_CC_DownloadAllCandidates]    
(    
@boardId int,  
@roundNo int  
)    
AS    
select * from App_CandidateProfile where boardId=@boardId order by rollno 
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownloadAllChoice]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[XApp_CC_DownloadAllChoice]    
(    
@boardId int,    
@roundNo int  
)    
AS    
select boardId,roundNo,rollNo,optNo,instituteId,programId,isValid from App_Choice where boardId=@boardId order by rollNo,optNo  
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownloadAllotment]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[XApp_CC_DownloadAllotment]
(
@boardId int,
@roundNo int
)
AS
BEGIN
select A.boardId,A.roundNo,A.rollNo,A.otherInfo,A.ranktypeId,A.rank,A.optionNo,A.instituteId,I.description InstituteName, programId,P.description ProgramName,A.streamId,A.groupId,A.seatTypeId,A.allottedCat,A.allottedQuota,A.seatGenderId,A.isWithdrawn,A.reportingStatus
	from App_Allotment A 
		inner join MD_Institute I on A.boardId=I.boardId and  A.instituteId=I.id 
		inner join MD_Program P on A.boardId=P.boardId and A.programId=P.id
where A.boardId=@boardId and roundNo=@roundNo
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownloadAllotmentStatistics]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[XApp_CC_DownloadAllotmentStatistics]
(
@boardId int,
@roundNo int
)
AS
BEGIN
select * from App_AllotmentSummary A
where A.boardId=@boardId and roundNo=@roundNo
END

GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownLoadAllotmentSummaryLetter]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[XApp_CC_DownLoadAllotmentSummaryLetter] --XApp_CC_DownLoadAllotmentSummaryLetter '138012121',1    
(        
@boardId int,      
@roundNo int      
)     
AS    
Begin    
 --(1) Heading    
 select header,subHeader from MD_CounsellingBoard where id=@boardId     
 --(2) Eligible Candidates    
 select count(*) total from XT_EligibleCandidate    
 --(3) Category Wise Candidates    
 select B.description+'('+ Category+ ')' Category,count(*) tot from XT_EligibleCandidate A inner join MD_Category B on A.Category=B.id group by category,B.description    
 --(4) Subcategory Wise Candidates    
 select Subcategory,count(*) tot from XT_EligibleCandidate group by Subcategory    
 --(5) Willingness wise candidates    
 select Willingness,count(*) tot from XT_EligibleCandidate group by Willingness    
 --(6) Symbol wise candidates    
 select Symbol,count(*) tot from XT_EligibleCandidate group by Symbol    
 --(7) Previous Allotment    
 select count(*) from XT_PreviousAllotment    
 --(8) Willingness of Previous Allotted Candidates     
 select Willingness,count(*) tot from XT_PreviousAllotment A inner join XT_EligibleCandidate B on A.Rollno=B.Rollno  group by B.Willingness    
 --(9) Virtual Choices    
 select count(*) from XT_VirtualChoice    
 --(10) Overall Allocation stats    
 select sum(initSeats) initialSeats,sum(NewSeats) FinalSeats,sum(allotted) Allotted,sum(NewSeats-allotted) Vacancy, sum(dereservefrom) DereservedFrom,sum(dereserveto) DereservedTo from XT_AllotmentSummary    
 --(11) New Allotment     
 select Count(*) newAllotment from XT_Allotment where rollno not in (select rollno from XT_PreviousAllotment)    
 --(12) Retainded      
 Select count(*) NoChange from XT_Allotment A inner join XT_PreviousAllotment B on A.RollNo=B.RollNo where A.instcd=B.instcd and A.brcd=B.brcd and A.Sequence=B.Sequence    
 --(13) Choice Upgradation    
 Select count(*) ChoiceUpgradation from XT_Allotment A inner join XT_PreviousAllotment B on A.RollNo=B.RollNo where A.instcd<>B.instcd or A.brcd<>B.brcd    
 --(14) Category Upgradation     
 Select count(*) CategoryUpgradation from XT_Allotment A inner join XT_PreviousAllotment B on A.RollNo=B.RollNo where A.instcd=B.instcd and A.brcd=B.brcd and  A.Sequence<>B.Sequence    
 --(15) Sequence Wise Seat Conversion     
 select A.Sequence,sum(initSeats) initialSeats,sum(NewSeats) FinalSeats,sum(initSeats-isnull(oldAllotted,0)) Offered, sum(allotted) Allotted,sum(NewSeats-allotted) Vacancy, sum(dereservefrom) DereservedFrom,sum(dereserveto) DereservedTo     
  from XT_AllotmentSummary A left outer join (select Instcd, Brcd, Sequence,count(*) oldAllotted from XT_PreviousAllotment group by Instcd, Brcd, Sequence) B    
   on A.instcd=B.instcd and A.brcd=B.Brcd and A.Sequence=B.Sequence    
   where A.InItSeats<>0 or A.NewSeats<>0 or A.Allotted<>0 or A.DereserveFrom<>0 or A.DereserveTo<>0    
  group by A.Sequence order by A.Sequence    
 --(16) Total Iteration     
 select isnull(max(iterationno),0) from XT_Allotted    
 --(17) Iteration Wise overall Seat Conversion     
 select iterationno,sum(seats) Seats,sum(allotted) allotted,sum(Dereservefrom) Dereservefrom,sum(DereserveTo) DereserveTo from Xt_Dereserve group by iterationno    
 --(18)  Iteration and sequence wise seat conversion    
 select iterationno,Sequence,sum(DereserveFrom) DereserveFrom,sum(DereserveTo) DereserveTo from Xt_Dereserve where DereserveFrom>0 or DereserveTo>0  group by iterationno,Sequence order by iterationno,Sequence    
 --(19) Current Round schedule     
 Select A.activityId,description,sDate,cDate,total from App_Schedule A inner join     
 (select boardId,activityId,count(*) total from App_ActivityStatus where roundNo=@roundNo group by boardId,activityId) B     
  on A.boardId=B.boardId and A.activityId=B.activityId     
 where roundNo=@roundNo    
 union    
 select activityId,description,sDate,cDate,(select count(distinct rollno) from App_Choice where  roundNo=@roundNo)  from App_Schedule where activityId='9' and roundNo=@roundNo    
 order by sDate,activityId    
    
 --(20) Previous Round Activities    
 Select A.activityId,description,sDate,cDate,total from App_Schedule A inner join     
 (select boardId,activityId,count(*) total from App_ActivityStatus where roundNo=@roundNo-1 group by boardId,activityId) B     
  on A.boardId=B.boardId and A.activityId=B.activityId     
 where roundNo=@roundNo-1    
 union    
 select activityId,description,sDate,cDate,(select count(distinct rollno) from App_Choice where  roundNo=@roundNo-1)  from App_Schedule where activityId='9' and roundNo=@roundNo-1    
 order by sDate,activityId    
    
 --(21) Quota,Category Wise, subcategory wise    
 select quotaId,B.Description Category, sum(allotted) Allotted,sum(InItSeats) InitSeats,sum(NewSeats) NewSeats,sum(NewSeats-Allotted) Vacancy     
  from App_AllotmentSummary  A inner join MD_SeatCatSubcatList B on A.categoryId+A.subcategoryId=B.catSubcat     
 where roundNo=@roundNo and InItSeats<>0 or NewSeats<>0 or allotted<>0 or DereserveFrom<>0 or DereserveTo<>0    
 group by quotaId,B.Description,B.priority    
 order by quotaId,B.priority    
    
 --(22) Institute Wise overall allocation Allocation    
 select instituteId, B.description InstituteName, sum(allotted) Allotted,sum(InItSeats) InitSeats,sum(NewSeats) NewSeats,sum(NewSeats-Allotted) Vacancy     
  from App_AllotmentSummary  A inner join MD_Institute B on A.instituteId=B.id     
 where roundNo=@roundNo    
 group by instituteId, B.description    
 order by instituteId, B.description    
     
 --(23)Opening and Closing Rank for Upgraded and Fresh Allocation     
 select A.instituteId,B.description InstituteName, A.programId,C.description programName,A.quotaId,D.description AllottedCat,OpeningRank_NewCand [OR],ClosingRank_NewCand CR    
 from App_AllotmentSummary A    
  inner join MD_Institute B on A.instituteId=B.id    
  inner join MD_Program C on A.programId=C.id    
  inner join MD_SeatCatSubcatList D on A.categoryId+A.subcategoryId=D.catSubcat    
 where roundNo=@roundNo and OpeningRank_NewCand<>0    
 order by A.InstituteId,A.programId,A.QuotaId,D.priority    
    
     
END    
  
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownloadEligibleCandidate]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[XApp_CC_DownloadEligibleCandidate]
(
@boardId int,
@roundNo int
)
AS
BEGIN
select [boardId], [roundNo], A.[rollno], [applicationNo], [name], [mName], [fName], [gName], [stateId], [domicileId], [genderId], [DOB], [categoryId], [subCategoryList], [subCategoryPriorityList], [nationalityId], [countryId], [otherCountryName], [otherInfo], [percentageOfDisability], [typeOfDisability], [categoryCertificateNo], [nclCertificateNo], [religionId],B.AdditionalInfo,B.Symbol,B.Rank,B.Willingness  from App_CandidateProfile A inner join XT_EligibleCandidate B on A.rollno=B.RollNo
where A.boardId=@boardId 
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownloadORCR]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[XApp_CC_DownloadORCR]  
(  
@boardId int,  
@roundNo int  
)  
As  
select instituteId,programId,streamId,allottedQuota,groupId,allottedCat,min(rank) OpeningRank,max(rank) closingRank from App_Allotment where boardId=@boardId and roundNo=@roundNo  
group by instituteId,programId,streamId,allottedQuota,groupId,allottedCat  
order by instituteId,programId,streamId,allottedQuota,groupId,allottedCat  

GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_DownloadPreviousAllotment]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[XApp_CC_DownloadPreviousAllotment]    
(    
@boardId int,  
@roundNo int  
)    
AS  
Select A.[boardId], [roundNo], A.[rollNo], [otherInfo], [ranktypeId], [rank], [instituteId], [programId], [streamId], [groupId], [seatTypeId], [allottedCat], [allottedQuota], [seatGenderId], [isWithdrawn], [reportingStatus], B.instcd,B.brcd,B.sequence,B.optNo,B.isRetained from App_Allotment A 
	inner join XT_PreviousAllotment B on A.rollno=B.rollNo
where A.roundNo=@roundNo-1
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_GetCandidateChoice]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[XApp_CC_GetCandidateChoice] 
(
@RollNo varchar(20),
@boardId int
)
as
Select optno,instituteId instcd,programId brcd,isValid from App_Choice where boardId=@boardId and rollno =@RollNo and isnull(isValid,'Y')='Y' 
order by Optno
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_GetCandidateChoiceAll]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[XApp_CC_GetCandidateChoiceAll] 
(
@boardId int,
@roundNo int,
@rollNo varchar(20)

)
as
Select optno,I.description+' ('+ instituteId+ ')' institute, P.description +' ('+  programId+')' Program,isnull(isValid,'Y') isValid from App_Choice A 
	inner join MD_Institute I on A.boardId=I.boardId and A.instituteId=I.id
	inner join MD_Program P on A.boardId=P.boardId and A.programId=P.id
	where A.boardId=@boardId and rollno =@RollNo 
order by Optno
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_GetSeatRankTypeMapping]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[XApp_CC_GetSeatRankTypeMapping]
(
@boardId int,
@roundNo int
)
AS
BEGIN 	
	select instituteId+'.'+programId+'.'+streamId+'.'+ GroupId+'.'+ SeatType+ '.'+QuotaId+'.'+categoryId+'.'+subcategoryId+'.'+genderId wlKey,streamId+'.'+rankTypeId rankTypeId  from App_Seat where boardId=@boardId and roundNo=@roundNo
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_GetSymbolCatSubcatOptions]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[XApp_CC_GetSymbolCatSubcatOptions]  
(  
@boardId int  
)  
AS  
select boardId,concat(isnull(symbolId,''),'.',isnull(categoryId,''),'.',isnull(subCategoryId,'')) CandCatSubcat  
                                ,CONCAT(left(availableCatSubCatOption,2),'.', right(availableCatSubcatOption,2)) availableCatSubCatOption,priority  
                                from MD_ChoiceCatSubcatOptions   
                                where boardId = @boardId 
								ORDER BY boardId,CandCatSubcat, priority 
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_GetUser]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[XApp_CC_GetUser]  
(  
 @boardId varchar(10),  
 @roundId int,  
 @userId varchar(50),  
 @password varchar(100),  
 @passwordSalt varchar(100)  
 )  
AS  
BEGIN  
 select name userName,defaultRoleId+ ','+ISNULL(AdditionalRoleIds,'') userRole  from App_Administrator   
  where (boardids like '%'+@boardId+'%' or agencyId='100')   
   and (defaultRoleId+ ','+ISNULL(AdditionalRoleIds,'') like '%BOARDADMIN%'   
     or defaultRoleId+ ','+ISNULL(AdditionalRoleIds,'') like '%NICADMIN%')   
   and isActive='Y' and  UserId=@userId and @password =CONVERT(NVARCHAR(64),HashBytes('sha2_256', password+ @passwordSalt),2);  
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_PrepareEligibleCandidate]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[XApp_CC_PrepareEligibleCandidate]  
(  
@boardId int,  
@roundNo int  
)  
As  
BEGIN  
 Truncate Table XT_EligibleCandidate;  
 insert into XT_EligibleCandidate  
 select C.rollno, C.categoryId, C.subCategoryList,genderId, domicileId domicileId, ('stateId='+isnull(stateId,'')) as AdditionalInfo,   
  null symbol,null  [Rank], (isnull(W.Willingness,'FL')) Decision  
  from App_CandidateProfile C  
   left outer join App_Willingness W on C.boardId=W.boardId and C.rollno=W.rollNo  
  where C.boardId=@boardId   
   and exists (Select 1 from App_QualMarkDetail where boardid=@boardId and rollno=C.rollno and  symbol!='N')  
   and Not exists (select 1 from App_Allotment where rollno=C.Rollno and   roundNo<@roundNo and (reportingStatus='NR' or iswithdrawn='Y'))  
   and exists (select 1 from App_Choice where rollno=C.Rollno and isnull(isValid,'Y')='Y' )  
  
   
 Declare @streamId varchar(2)  
 Declare c Cursor For Select Distinct streamId From App_QualMarkDetail where boardId=@boardId order by streamId  
 Open c  
 Fetch next From c into @streamId  
 While @@Fetch_Status=0   
 Begin  
    update A Set A.Symbol=CONCAT(isnull(A.symbol,''), ',',Q.streamId,'=', Q.symbol)    
   from XT_EligibleCandidate A   
      inner join App_QualMarkDetail Q on Q.boardId=@boardId and Q.streamId=@streamId  and A.rollNo=Q.rollNo  
    Fetch next From c into @streamId  
 End  
 Close c  
 Deallocate c  
  
 Declare @rankTypeId varchar(2)  
 Declare d Cursor For Select Distinct streamId,rankTypeId From App_RankDetail where boardId=@boardId order by streamId,rankTypeId  
 Open d  
 Fetch next From d into @streamId,@rankTypeId  
 While @@Fetch_Status=0   
 Begin  
    update A Set A.Rank=CONCAT(isnull(A.Rank,''), ',',R.streamId,'.',R.rankTypeId,'=',case when  Q.symbol='N' or R.rank is null then '' else ltrim(STR(R.Rank, 25, 5)) end)   
   from XT_EligibleCandidate A   
      inner join App_RankDetail R on R.boardId=@boardId and A.RollNo=R.rollNo  
      inner join App_QualMarkDetail Q on R.boardId=Q.boardId and R.streamId=Q.streamId  and R.rollNo=Q.rollNo    
    where R.streamId=@streamId and R.rankTypeId=@rankTypeId   and Q.symbol!='N'
    Fetch next From d into @streamId,@rankTypeId  
 End  
 Close d  
 Deallocate d  
 update XT_EligibleCandidate set Symbol= SUBSTRING(Symbol,2,len(symbol)-1) where Symbol is not null  
 update XT_EligibleCandidate set Rank= SUBSTRING(rank,2,len(Rank)-1) where Symbol is not null  
END  
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_PreparePreviousAllotment]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[XApp_CC_PreparePreviousAllotment] 
(
@boardId int,
@roundNo int
)
As
BEGIN
	truncate table XT_PreviousAllotment;
	insert into XT_PreviousAllotment
	select A.rollNo,A.instituteId,A.programId,CONCAT(A.streamId,'.', A.groupId,'.',A.seatTypeId,'.' ,A.allottedQuota,'.', left(A.allottedCat,2),'.',right(A.allottedCat,2),'.',A.seatGenderId) Sequence, Ch.optNo,'1' 
		from app_allotment A 
			inner join App_Choice Ch on A.boardId=Ch.boardId and A.roundNo=@roundNo-1 and A.rollNo=Ch.rollNo and A.instituteId=Ch.instituteId And A.programId=Ch.ProgramId and A.boardId=Ch.boardId
		where A.boardId=@boardId and A.reportingStatus in ('RP','RT','RU','CU') and isWithdrawn='N'
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_PrepareSeat]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[XApp_CC_PrepareSeat]
(
@boardId int,
@roundNo int
)
AS
BEGIN
truncate table XT_Seat;
Insert into XT_Seat
select instituteId, programId, streamId+'.'+ GroupId+'.'+ SeatType+ '.'+QuotaId+'.'+categoryId+'.'+subcategoryId+'.'+genderId,NULL stateIdList, tSeat  from App_Seat where boardId=@boardId and roundNo=@roundNo
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_UpdateApplication]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[XApp_CC_UpdateApplication]
(
@RoundNo int,
@BoardId int,
@dtAllotment XApp_CC_AllotmentTable READONLY,
@dtAllotmentSummary XApp_CC_AllotmentSummaryTable READONLY
)
as
BEGIN
	Insert Into App_Allotment
	select boardId, roundNo, rollNo, null otherInfo, ranktypeId, rank, 0 optionNo, instituteId, programId, streamId,groupId,seatTypeId,allottedCat,
	allottedQuota,seatGenderId,'N' isWithdrawn,'NR' reportingStatus,'Allotted' allotmentHistory,getdate() allocationDate,null PIStattus from @dtAllotment
	
	update A set otherInfo=b.categoryId+','+b.subCategoryList,A.optionNo=C.optNo  from App_Allotment A 
		inner join App_CandidateProfile B on A.boardId=B.boardId and A.rollNo=B.rollno and A.roundNo=@RoundNo
		inner join App_Choice C on A.boardId=C.boardId and A.rollNo=C.rollno and A.instituteId=C.instituteId and A.programId=C.programId
		where A.boardId=@BoardId and A.roundNo=@RoundNo
	
	if (@RoundNo>1)
	BEGIN
		--Update Previous Allotment history of all candidates
		Update N Set N.allocationHistory=P.allocationHistory,N.PIStatus=P.PIStatus from App_Allotment N 
			inner join (Select O.rollNo,O.allocationHistory,O.PIStatus from  App_Allotment O  inner join (select rollNo,max(Roundno) roundno from App_Allotment where roundNo<@RoundNo group By rollNo) I on O.rollNo=I.rollNo and O.roundNo=I.roundno ) P
			on N.roundNo=@RoundNo and  N.rollNo=P.rollNo 
    
		--RT   
		update N Set N.reportingStatus='RT' ,N.allocationHistory=N.allocationHistory+';RT' from App_Allotment N 
		inner join XT_PreviousAllotment O on N.roundNo=@RoundNo and  N.rollNo=O.rollno 
		where N.instituteId=O.Instcd and N.programId=O.Brcd and N.streamId+'.'+N.groupId+'.'+N.seatTypeId+'.'+N.allottedQuota+'.'+left(N.allottedCat,2)+'.'+RIGHT(N.allottedCat,2)+'.'+N.seatGenderId=O.Sequence 

		--RU	
		update N Set N.reportingStatus='RU' ,N.allocationHistory=N.allocationHistory+';RU' from App_Allotment N 
		inner join XT_PreviousAllotment O on N.roundNo=@RoundNo and  N.rollNo=O.rollno 
		where  N.instituteId<>O.Instcd or N.programId<>O.Brcd or N.streamId+'.'+N.groupId+'.'+N.seatTypeId+'.'+N.allottedQuota+'.'+left(N.allottedCat,2)+'.'+RIGHT(N.allottedCat,2)+'.'+N.seatGenderId<>O.Sequence  
  	END
	Insert into App_AllotmentSummary
	select @boardid,@RoundNo,instituteId,programId,streamId,groupId, seatType,quotaId,categoryId,subcategoryId,genderId,rankTypeId, InItSeats,NewSeats,Allotted,OpeningRank,ClosingRank,OpeningRank_NewCand,ClosingRank_NewCand,DereserveFrom,DereserveTo From @dtAllotmentSummary
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_ValidateCandidateProfile]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[XApp_CC_ValidateCandidateProfile]
(
@VerificaitonShortCode varchar(100),
@boardId varchar(10),
@roundId int,
@result int output,
@message varchar(500) output
)
AS
BEGIN
BEGIN TRY
Declare @Passed int = 5, @NotApplicable int= 1, @NotImplemented int= 2, @NotChecked int =0,@Error int=4,@Failed int= 3
	Set @result=@NotChecked;
	Set @message='';
	Declare @Rollno varchar(20)
	--Candidate Category Rules
	if(@VerificaitonShortCode='DistinctCategory')
	BEGIN
		select top 1 @Rollno=A.rollno from App_CandidateProfile A where boardId=@boardId and (categoryId is null or not exists (select 1 from MD_Category where boardId=@boardId and id=A.categoryId))
		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END
	--Candidate Subcategory Rules
	if(@VerificaitonShortCode='DistinctSubcategory')
	BEGIN
		SET @result=@Passed
		SET @message=''
		Declare @subcat varchar(200)
		Declare  curSubcat cursor for 
		select distinct subcategoryList from App_CandidateProfile
		open curSubcat
		fetch next from curSubcat into @subcat
		while @@FETCH_STATUS=0
		BEGIN
			if exists(select 1 from string_split(@subcat,',') where left(value,2) not in (select id from MD_SubCategory))
			BEGIN
				SET @result=@Failed   
				Set @message=@subcat
				break;
			END
			else if exists(select * from string_split(@subcat,',') where substring(value,4,10) not in ('Yes','No'))
			BEGIN	
				SET @result=@Failed   
				Set @message=@subcat
				break;
			END
			fetch next from curSubcat into @subcat
		END
		close curSubcat
		Deallocate curSubcat
		return
	END
	--Candidate Domicile Rules
	--Candidate Gender Rules
	if(@VerificaitonShortCode='DistinctGender')
	BEGIN
		select top 1 @Rollno=A.rollno from App_CandidateProfile A where boardId=@boardId and (genderId is null or not exists (select 1 from MD_Gender where boardId=@boardId and id=A.genderId))
		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END

	if(@VerificaitonShortCode='SymbolNChoiceFilling')
	BEGIN
		--select top 1 @rollNo=rollno from App_QualMarkDetail where symbol='N' and exists(select 1 from App_Choice where rollNo=App_QualMarkDetail.rollNo and isValid is null)
		 select top 1 @rollNo=A.rollno from App_QualMarkDetail A     
			inner join App_Seat S on A.boardId=S.boardId and A.streamId=S.StreamId      
			inner join App_Choice C on S.boardId=C.boardId and A.rollNo=C.rollNo and S.instituteId=C.instituteId and s.programId=C.programId    
			where A.boardId=@boardId and A.symbol='N' and isValid is null
		
		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END
	
	Set @result=@NotImplemented
	Set @message=''
END TRY
BEGIN CATCH	
	Set @result=@Error
	Set @message=''
END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_ValidateChoice]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[XApp_CC_ValidateChoice]
(
@VerificaitonShortCode varchar(100),
@boardId varchar(10),
@roundId int,
@result int output,
@message varchar(500) output
)
AS
BEGIN
BEGIN TRY
Declare @Passed int = 5, @NotApplicable int= 1, @NotImplemented int= 2, @NotChecked int =0,@Error int=4,@Failed int= 3
	Set @result=@NotChecked;
	Set @message='';
	Declare @Rollno varchar(20)
	/*
	if(@VerificaitonShortCode='RegFee')
	BEGIN
		select top 1 @Rollno=rollno from App_Choice where not exists (select 1 from App_ActivityStatus where rollNo=App_Choice.rollNo and activityId='08') 

		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END
	*/
		
	Set @result=@NotImplemented
	Set @message=''
END TRY
BEGIN CATCH	
	Set @result=@Error
	Set @message=''
END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_ValidateInput]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[XApp_CC_ValidateInput]
(
@VerificaitonShortCode varchar(100),
@boardId varchar(10),
@roundId int,
@result int output,
@message varchar(500) output
)
AS
BEGIN
BEGIN TRY
Declare @Passed int = 5, @NotApplicable int= 1, @NotImplemented int= 2, @NotChecked int =0,@Error int=4,@Failed int= 3
	Set @result=@NotChecked;
	Set @message='';
	Declare @Rollno varchar(20)
	/*
	if(@VerificaitonShortCode='RegFee')
	BEGIN
		select top 1 @Rollno=rollno from App_Choice where not exists (select 1 from App_ActivityStatus where rollNo=App_Choice.rollNo and activityId='08') 

		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END
	*/
		
	Set @result=@NotImplemented
	Set @message=''
END TRY
BEGIN CATCH	
	Set @result=@Error
	Set @message=''
END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_ValidateOutput]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Proc [dbo].[XApp_CC_ValidateOutput]
(
@VerificaitonShortCode varchar(100),
@boardId varchar(10),
@roundId int,
@result int output,
@message varchar(500) output
)
AS
BEGIN
BEGIN TRY
Declare @Passed int = 5, @NotApplicable int= 1, @NotImplemented int= 2, @NotChecked int =0,@Error int=4,@Failed int= 3
	Set @result=@NotChecked;
	Set @message='';
	Declare @Rollno varchar(20)

	if(@VerificaitonShortCode='AllotOpenSymbolStar')
	BEGIN
		select top 1 @Rollno=A.rollno from App_Allotment A inner join App_CandidateProfile B on A.rollNo=B.rollno and A.roundNo= and allottedCat='OPNO'
	where A.rollNo not in (select rollno from App_QualMarkDetail where symbol='*')
		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END

	if(@VerificaitonShortCode='AllotN')
	BEGIN
		select top 1 @Rollno=rollno from App_Allotment A where boardId=@boardId and roundNo=@roundId  
		and exists( select 1 from App_QualMarkDetail where boardId=A.boardId and roundNo=A.roundNo and rollNo=A.Rollno and streamId=A.streamId and symbol='N')  

		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END
	
	if(@VerificaitonShortCode='CatSeatCandCat')
	BEGIN
		select top 1 @Rollno=A.rollno from App_Allotment A inner join App_CandidateProfile B on A.rollNo=B.rollno and A.roundNo=@roundId 
	 where (left(A.allottedCat,2)not in('OP','NO') and left(allottedcat,2)!=B.categoryId)  

		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END

	if(@VerificaitonShortCode='PwDSeatsCandPwD')
	BEGIN
		select top 1 @Rollno=A.rollno from App_Allotment A inner join App_CandidateProfile B on A.rollNo=B.rollno and A.roundNo=@roundId 
	where (right(A.allottedCat,2)='PH' and B.subCategoryList not like '%PH:Yes%')

		if(@Rollno is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@Rollno
		END
		return
	END
		
	Set @result=@NotImplemented
	Set @message=''
END TRY
BEGIN CATCH	
	Set @result=@Error
	Set @message=''
END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[XApp_CC_ValidateSeat]    Script Date: 14-08-2021 13:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[XApp_CC_ValidateSeat]
(
@VerificaitonShortCode varchar(100),
@boardId varchar(10),
@roundId int,
@result int output,
@message varchar(500) output
)
AS
BEGIN
BEGIN TRY
Declare @Passed int = 5, @NotApplicable int= 1, @NotImplemented int= 2, @NotChecked int =0,@Error int=4,@Failed int= 3
	Set @result=@NotChecked;
	Set @message='';
	Declare @instituteId varchar(10),@programId varchar(10),@seatCategoryId varchar(10),@seatSubcategoryId varchar(10),@seatGenderId varchar(10), @SeatGroup varchar(10),@SeatType varchar(10)

	if(@VerificaitonShortCode='DistinctInstitute')
	BEGIN
		select top 1 @instituteId=A.instituteId from App_Seat A where boardId=@boardId and (instituteId is null or not exists (select 1 from MD_Institute where boardId=@boardId and id=A.instituteId))
		if(@instituteId is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@instituteId
		END
		return
	END
	-- Branch Rules
	if(@VerificaitonShortCode='DistinctProgram')
	BEGIN
		select top 1 @programId=A.programId from App_Seat A where boardId=@boardId and (programId is null or not exists (select 1 from MD_Program where boardId=@boardId and id=A.programId))
		if(@programId is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@programId
		END
		return
	END
	-- Category Rules
	if(@VerificaitonShortCode='DistinctSeatCategory')
	BEGIN
		select top 1 @seatCategoryId=A.categoryId from App_Seat A where boardId=@boardId and categoryId!='No' and (categoryId is null or not exists (select 1 from MD_SeatCategory where boardId=@boardId and id=A.categoryId))
		if(@seatCategoryId is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@seatCategoryId
		END
		return
	END
	-- Subcategory Rule
	if(@VerificaitonShortCode='DistinctSeatSubcategory')
	BEGIN
		select top 1 @seatSubcategoryId=A.subcategoryId from App_Seat A where boardId=@boardId and (subcategoryId is null or (subcategoryId!='NO' and not exists (select 1 from MD_SeatSubCategory where boardId=@boardId and id=A.subcategoryId)))
		if(@seatSubcategoryId is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@seatSubcategoryId
		END
		return
	END
	-- Gender Rules
	if(@VerificaitonShortCode='DistinctSeatGender')
	BEGIN
		select top 1 @seatGenderId=A.genderId from App_Seat A where boardId=@boardId and (genderId is null or not exists (select 1 from MD_SeatGender where boardId=@boardId and id=A.genderId))
		if(@seatGenderId is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@seatGenderId
		END
		return
	END
	--Distinct Group
	if(@VerificaitonShortCode='DistinctSeatGroup')
	BEGIN
		select top 1 @SeatGroup=A.groupId from App_Seat A where boardId=@boardId and (groupId is null or not exists (select 1 from MD_SeatGroup where boardId=@boardId and groupId=A.groupId))
		if(@SeatGroup is null)
		BEGIN
			SET @result=@Passed
		END
		ELSE
		BEGIN
			SET @result=@Failed 
			Set @message=@SeatGroup
		END
		return
	END
	Set @result=@NotImplemented
	Set @message=''
END TRY
BEGIN CATCH	
	Set @result=@Error
	Set @message=''
END CATCH
END
GO


CREATE Proc [dbo].[XApp_CC_InstituteProgramStreamMaster] 
(
@boardId int,
@roundNo int
)
AS
	select instituteId+'.'+ programId instituteProgram,  STRING_AGG(streamId,',') StreamId 
	from (select distinct instituteId,programid,streamid from App_Seat where boardId=@boardId and roundNo=@roundNo ) A  group by instituteId,programid
GO

CREATE Proc [dbo].[XApp_CC_InstituteProgramSeatTypeMaster] 
(
@boardId int,
@roundNo int
)
AS
	select instituteId+'.'+ programId instituteProgram,  STRING_AGG(SeatType,',') SeatTypeId
	from (select distinct instituteId,programid,SeatType from App_Seat where boardId=@boardId and roundNo=@roundNo ) A  group by instituteId,programid
GO


Create Proc [dbo].[XApp_CC_InstituteProgramGroupMaster] 
(
@boardId int,
@roundNo int
)
AS
	select instituteId+'.'+ programId instituteProgram,  STRING_AGG(groupId,',') GroupId
	from (select distinct instituteId,programid,groupId from App_Seat where boardId=@boardId and roundNo=@roundNo ) A  group by instituteId,programid
GO


Create Proc [dbo].[XApp_CC_InstituteProgramQuotaMaster] 
(
@boardId int,
@roundNo int
)
AS
	select instituteId+'.'+ programId instituteProgram,  STRING_AGG(quotaId,',') QuotaId
	from (select distinct instituteId,programid,quotaId from App_Seat where boardId=@boardId and roundNo=@roundNo ) A  group by instituteId,programid
GO

Create Proc [dbo].[XApp_CC_InstituteProgramGenderMaster] 
(
@boardId int,
@roundNo int
)
AS
	select instituteId+'.'+ programId instituteProgram,  STRING_AGG(genderId,',') GenderId
	from (select distinct instituteId,programid,genderId from App_Seat where boardId=@boardId and roundNo=@roundNo ) A  group by instituteId,programid
GO
