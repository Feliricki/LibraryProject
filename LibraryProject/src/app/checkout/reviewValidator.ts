import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function reviewFormValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as number | null;

    if (value === null) return { valueRequired: true };

    // NOTE:Check if the value is a whole number
    if (value > 0 && value <= 5 && Number.isInteger(value)){
      return null;
    } else {
      return { invalidNumber: true }
    }
  }
}
