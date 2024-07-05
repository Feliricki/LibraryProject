import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthenticationService } from "../authentication.service";
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {LoginResult} from "../login-result";
import {LoginRequest} from "../login-request";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule, MatCardModule, MatButtonModule,
    MatFormFieldModule, MatInputModule, MatIconModule, RouterLink
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginResult?: LoginResult;
  hide: boolean = true;
  // TODO: Consider adding frontend validation
  loginForm = this.formBuilder.nonNullable.group({
    username: this.formBuilder.nonNullable.control("", Validators.required),
    password: this.formBuilder.nonNullable.control("", Validators.required)
  });

  constructor(
    private router: Router,
    private authService: AuthenticationService,
    private formBuilder: FormBuilder
  ) {
  }

  get Username() {
    return this.loginForm.controls.username;
  }

  get Password() {
    return this.loginForm.controls.password;
  }

  formHasErrors(): boolean {
    return this.Password.errors !== null || this.Username.errors !== null;
  }

  submitForm() {
    const loginRequest: LoginRequest = {
      username: this.Username.value,
      password: this.Password.value,
    };

    this.authService
      .login(loginRequest)
      .subscribe({
        next: response => {
          console.log(response);
          if (response.success){
            this.router.navigate(["/"]);
          }
        },
        error: err => {
          console.error(err);
          if (err.status === 401){
            this.loginResult = err.error;
          }
        }
      })
  }
}
