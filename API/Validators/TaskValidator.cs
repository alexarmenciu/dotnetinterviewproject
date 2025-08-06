using FluentValidation;
using API.Models;

namespace API.Validators
{
    public class TaskValidator : AbstractValidator<API.Models.Task>
    {
        public TaskValidator()
        {

            RuleFor(task => task.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .Length(1, 200)
                .WithMessage("Title must be between 1 and 200 characters.");

            RuleFor(task => task.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .Length(1, 1000)
                .WithMessage("Description must be between 1 and 1000 characters.")
                .Must(desc => !string.IsNullOrWhiteSpace(desc))
                .WithMessage("Description cannot be only whitespace.");

            RuleFor(task => task.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required.")
                .Must(dueDate => dueDate.Date >= DateTime.Now.Date)
                .WithMessage("Due date cannot be in the past.");

        }
    }
}
