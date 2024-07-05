import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthenticationService } from "../authentication.service";
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {SignupResult} from "../signup-result";
import { MatCheckboxModule } from '@angular/material/checkbox';
import {SignupRequest} from "../signup-request";

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [
    ReactiveFormsModule, MatCardModule, MatButtonModule,
    MatFormFieldModule, MatInputModule, MatIconModule, RouterLink,
    MatCheckboxModule
  ],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  signupResult?: SignupResult;
  hide: boolean = true;

  signupForm = this.formBuilder.nonNullable.group({
    username: this.formBuilder.nonNullable.control("", Validators.required),
    email: this.formBuilder.nonNullable.control("", Validators.email),
    password: this.formBuilder.nonNullable.control("", Validators.required),
    isLibrarian: this.formBuilder.nonNullable.control(false, Validators.required),
  });

  constructor(
    private router: Router,
    private authService: AuthenticationService,
    private formBuilder: FormBuilder
  ) {
  }

  get Username(){
    return this.signupForm.controls.username;
  }
  get Email() {
    return this.signupForm.controls.email;
  }
  get Password() {
    return this.signupForm.controls.password;
  }
  get IsLibrarian() {
    return this.signupForm.controls.isLibrarian;
  }

  submitForm() {
    const request: SignupRequest = {
      username: this.Username.value,
      email: this.Email.value,
      password: this.Password.value,
      isLibrarian: this.IsLibrarian.value,
    };

    this.authService
      .signup(request)
      .subscribe({
        next: response => {
          console.log(response);
          if (response.success){
            this.router.navigate(['/']);
          }
        },
        error: err => {
          console.error(err);
          if (err.status === 401 || err.status == 400){
            this.signupResult = err.error;
          }
        }
      })
  }
}
