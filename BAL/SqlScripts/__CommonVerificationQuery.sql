select * from XT_EligibleCandidate
select count(*) from XT_EligibleCandidate
select distinct RollNo from XT_EligibleCandidate
select * from XT_EligibleCandidate where RollNo is null or [Rank] is null or Category is null or Subcategory is null or Symbol is null or AdditionalInfo is null
select distinct Category from XT_EligibleCandidate
select distinct Subcategory from XT_EligibleCandidate
select distinct Willingness from XT_EligibleCandidate
select distinct Symbol from XT_EligibleCandidate
select * from XT_EligibleCandidate where [rank] is null
select willingness,count(*) from XT_EligibleCandidate group by willingness
select Category,count(*) from XT_EligibleCandidate group by Category
select Subcategory,count(*) from XT_EligibleCandidate group by Subcategory


select sum(tseat) from XT_Seat 
select * from XT_Seat where Instcd is null or BrCd is NUll or  Sequence is null 

select count(*) from XT_VirtualChoice 
Select count(*) from XT_EligibleCandidate where Rollno not in (select rollno from XT_VirtualChoice)
select count(*) from XT_PreviousAllotment  where Rollno in (select rollno from XT_EligibleCandidate)
select count(*) from XT_PreviousAllotment  where Rollno  in (select rollno from XT_VirtualChoice)

select count(*) from XT_Allotment 
select count(*) from XT_Allotment
select count(*) from XT_PreviousAllotment where rollno  in (select rollno from XT_Allotted where iterationno=1 )
select count(*) from XT_PreviousAllotment where rollno  in (select rollno from XT_Allotment)
select count(*) from XT_Allotment where rollno  in (select rollno from XT_PreviousAllotment)

select max(iterationno) from XT_Allotted
select iterationno,sum(seats),sum(allotted),sum(Dereservefrom),sum(DereserveTo) from Xt_Dereserve group by iterationno
select iterationno,Sequence,sum(DereserveFrom),sum(DereserveTo) from Xt_Dereserve where DereserveFrom>0 or DereserveTo>0  group by iterationno,Sequence   
order by iterationno,Sequence
---If Allotment is without conversion
select  * from XT_Seat S 
	Full outer join XT_PSeat P on S.instcd=P.InstCd and S.BrCd=P.BrCd and S.Sequence=P.Sequence
	where S.Tseat is null or P.TSeat is null or S.TSeat<>P.TSeat

--Check if Seat varies during iteration
select * from 
(select instcd,brcd,sum(seats) seats from XT_Dereserve where iterationno=2 group by instcd,brcd) A
inner join (select instcd,brcd,sum(seats) seats from XT_Dereserve where iterationno=3 group by instcd,brcd) B on A.instcd=B.instcd and A.brcd=B.brcd
and A.Seats<>b.seats

select * from XT_AllotmentSummary
select sum(initSeats),sum(NewSeats),sum(allotted),sum(NewSeats-allotted), sum(dereservefrom),sum(dereserveto) from XT_AllotmentSummary
select * from XT_AllotmentSummary where newseats<>initseats+DereserveTo-dereserveFrom 
select * from XT_AllotmentSummary where newSeats<Allotted
select * from XT_AllotmentSummary where initSeats<Allotted


--------------Split Addional info parts
;WITH Split_Names (RollNo,AdditionalInfo, xmlname)
AS
(
    SELECT RollNo,
    AdditionalInfo,
    CONVERT(XML,'<Names><name>'+ REPLACE(AdditionalInfo,'|', '</name><name>') + '</name></Names>') AS xmlname
      FROM XT_EligibleCandidate
)
--SELECT RollNo,AdditionalInfo , xmlname.value('/Names[1]/name[1]','varchar(100)') AS DegreeType, xmlname.value('/Names[1]/name[2]','varchar(100)') AS GateExamPaperCode, xmlname.value('/Names[1]/name[3]','varchar(100)') AS Degree FROM Split_Names
--SELECT distinct(xmlname.value('/Names[1]/name[1]','varchar(100)')) AS DegreeType FROM Split_Names
--SELECT distinct(xmlname.value('/Names[1]/name[2]','varchar(100)')) AS GateExamPaperCode FROM Split_Names
SELECT distinct(xmlname.value('/Names[1]/name[3]','varchar(100)')) AS Degree FROM Split_Names
 ---------------------End of Split------------------


 Select * from XT_EligibleCandidate A 
	inner join XT_PreviousAllotment B on A.RollNo=B.RollNo
	inner join XT_Allotment C on A.RollNo=C.RollNo
	inner join App_Choice D on A.RollNo=D.rollNo and C.Instcd=D.instituteId and C.Brcd=D.programId
where A.Willingness='FR' and B.Instcd!=C.Instcd and B.Brcd!=C.Brcd

 Select * from XT_EligibleCandidate A 
	inner join XT_PreviousAllotment B on A.RollNo=B.RollNo
	inner join XT_Allotment C on A.RollNo=C.RollNo
	inner join App_Choice D on A.RollNo=D.rollNo and C.Instcd=D.instituteId and C.Brcd=D.programId
where A.Willingness='FR' and B.Instcd=C.Instcd and B.Brcd=C.Brcd

Select * from XT_EligibleCandidate A 
	inner join XT_PreviousAllotment B on A.RollNo=B.RollNo
	inner join XT_Allotment C on A.RollNo=C.RollNo
	inner join App_Choice D on A.RollNo=D.rollNo and C.Instcd=D.instituteId and C.Brcd=D.programId
where A.Willingness='FL' and B.OptNo<D.optNo

Select * from XT_EligibleCandidate A 
	inner join XT_PreviousAllotment B on A.RollNo=B.RollNo
	inner join XT_Allotment C on A.RollNo=C.RollNo
	inner join App_Choice D on A.RollNo=D.rollNo and C.Instcd=D.instituteId and C.Brcd=D.programId
where A.Willingness='FL' and B.OptNo>=D.optNo


select * from XT_EligibleCandidate where Category!='BT' 
	and rollno in (select rollno from App_Choice where 
		instituteId+programId in (select instituteId+programId from App_AllotmentSummary where InItSeats-Allotted>0 and  quotaid+categoryId+subcategoryId='HSOPNO'))
		and rollno not  in (select rollno from App_Allotment where roundNo=3)

select * from XT_EligibleCandidate where Category!='BT' 
	and rollno in (select rollno from App_Choice where 
		instituteId+programId in (select instituteId+programId from App_AllotmentSummary where InItSeats-Allotted>0 and  quotaid+categoryId+subcategoryId='HSOPNO'))
		and rollno in (select rollno from App_Allotment where roundNo=3)

select * from XT_EligibleCandidate A inner join App_Allotment B on A.RollNo=B.rollNo where B.roundNo=3 and Category!='BT' 
	and exists  (select 1 from App_Choice where rollNo=A.RollNo and B.optionNo<=optionNo and
		instituteId+programId in (select instituteId+programId from App_AllotmentSummary where InItSeats-Allotted>0 and  quotaid+categoryId+subcategoryId='HSOPNO'))

