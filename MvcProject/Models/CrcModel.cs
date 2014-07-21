using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MvcProject.Models
{
    [FluentValidation.Attributes.Validator(typeof(CrcValidator))]
    public class CrcModel
    {
        public int ID { get; set; }

        [Display(Name = "Binary value")]
        public string binaryValue { get; set; }

        [Display(Name = "Generator")]
        public string generator { get; set; }

        [Display(Name = "Remainder")]
        public string remainder { get; set; }

        [Display(Name = "Signal")]
        public string signal { get; set; }

        [Display(Name = "Correctness")]
        public string correctness { get; set; }

        public DateTime DateAdded { get; set; }

        public virtual ApplicationUser user { get; set; }
    }


    public class CrcValidator : AbstractValidator<CrcModel>
    {
        public CrcValidator()
        {
            RuleFor(x => x.binaryValue)
                .NotEmpty().WithMessage("Binary value is required")
                .Matches(@"(0|1)*").WithMessage("Binary value must be in binary format");

            RuleFor(x => x.generator)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Generator is required")
                .Matches(@"(0|1)*").WithMessage("Generator must be in binary format")
                .Must(CompareLength).WithMessage("Generator must be shorter than Binary value");
        }

        private bool CompareLength(CrcModel model, string value)
        {
            return model.binaryValue.Length > model.generator.Length;
        }
    }
}