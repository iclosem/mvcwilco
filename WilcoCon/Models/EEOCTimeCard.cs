using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WilcoCon.Models
{
    public class EEOCTimeCard
    {
        private WilcoEntities db = new WilcoEntities();
        public string Date;
        public int ProjectId;
        public string ProjectNumber;
        public string ProjectLocation;
        public string ProjectDescription;
        public string SkillCode { get; set; }
        public double MaleMinorityHours { get; set; }
        public double FemaleMinorityHours { get; set; }
        public double MaleNonMinorityHours { get; set; }
        public double FemaleNonMinorityHours { get; set; }
        public double JobHours { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public double PercentageJobHoursWorkedMinority { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public double PercentageJobHoursWorkedFemale { get; set; }
        public List<EEOCTimeCard> AllHoursWorked(Project project, DateTime endDate)
        {
            Repository repository = new Repository();
            var JobClassifications = repository.AllJobClassifications();
            List<EEOCTimeCard> eeocTimeCardList= new List<EEOCTimeCard>();
            
            foreach (JobClassification skill in JobClassifications)
            {
                EEOCTimeCard eeoctimecard = new EEOCTimeCard();
                eeoctimecard.SkillCode = skill.SkillCode;
                //HoursWorked is calculating correctly but the sumHours is not being passed properly.
                eeoctimecard.MaleMinorityHours = repository.HoursWorked(project, "Male", 1, skill.SkillCode, endDate).TotalHours;
                eeoctimecard.FemaleMinorityHours = repository.HoursWorked(project, "Female", 1, skill.SkillCode, endDate).TotalHours;
                eeoctimecard.MaleNonMinorityHours = repository.HoursWorked(project, "Male", 0, skill.SkillCode, endDate).TotalHours;
                eeoctimecard.FemaleNonMinorityHours = repository.HoursWorked(project, "Female", 0, skill.SkillCode, endDate).TotalHours;
                eeoctimecard.JobHours = repository.HoursWorked(project, skill.SkillCode, endDate).TotalHours;
                if (eeoctimecard.JobHours != 0)
                {
                    eeoctimecard.PercentageJobHoursWorkedMinority = (eeoctimecard.MaleMinorityHours + eeoctimecard.FemaleMinorityHours) / eeoctimecard.JobHours;
                }
                else
                {
                    eeoctimecard.PercentageJobHoursWorkedMinority = 0;
                }
                double totalJobHours = eeoctimecard.JobHours;
                if (eeoctimecard.JobHours != 0)
                {
                    eeoctimecard.PercentageJobHoursWorkedFemale = (eeoctimecard.FemaleMinorityHours + eeoctimecard.FemaleNonMinorityHours) / eeoctimecard.JobHours;
                }
                else
                {
                    eeoctimecard.PercentageJobHoursWorkedFemale = 0;
                }
                eeoctimecard.Date = repository.GetDate(project);
                eeoctimecard.ProjectLocation = project.ProjectLocation;
                eeoctimecard.ProjectDescription = project.ProjectDescription;
                eeoctimecard.ProjectNumber = project.ProjectNumber;
                eeoctimecard.ProjectId = project.ProjectId;
                eeocTimeCardList.Add(eeoctimecard);
            }
            return eeocTimeCardList;
        }
         

    }
    
}