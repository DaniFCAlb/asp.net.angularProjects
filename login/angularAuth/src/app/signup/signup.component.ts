import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import ValidateForm from '../helpers/validateform';

import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../services/auth.service';


@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  type: string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";
  signupForm!: FormGroup;

  constructor(private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService) {

  }

  ngOnInit(): void {
    this.signupForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      username: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', Validators.required]
    })
  }


  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onSignup() {
    if(this.signupForm.valid) {
      //send to db

      this.auth.signUp(this.signupForm.value).subscribe({
        next:(res) => {
          this.signupForm.reset();
          this.toastr.success("Succes", res.message, {timeOut: 5000});
          this.router.navigate(['login']);
        },
        error:(err) => {
          this.toastr.error("Something went wrong" ,"ERROR", {timeOut: 5000});
          console.log(err);
        }
      });
    }else {
      //throw error using toastr and rquired fields
      ValidateForm.validateAllFormFields(this.signupForm);

    }
  }
}
