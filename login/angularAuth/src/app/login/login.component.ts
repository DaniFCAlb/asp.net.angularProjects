import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { timeout } from 'rxjs';


import ValidateForm from '../helpers/validateform';
import { AuthService } from '../services/auth.service';
import { UserStoreService } from '../services/user-store.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  type:string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";
  loginForm!: FormGroup;

  constructor(private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService,
    private userStore: UserStoreService) {

  }

  ngOnInit(): void {
    this.loginForm = this.fb.group( {
      username: ['', Validators.required],
      password: ['', Validators.required]
    })
  }


  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onLogin() {
    if(this.loginForm.valid) {
      //send to db
      console.log(this.loginForm.value);
      this.auth.login(this.loginForm.value).subscribe({
        next:(res) => {
          console.log(res.message)
          this.loginForm.reset();
          this.auth.storeToken(res.token);

          const tokenPayload = this.auth.decodedToken();
          this.userStore.setFullNameForStore(tokenPayload.name);
          this.userStore.setRoleForStore(tokenPayload.role);
          
          this.toastr.success("Succes", res.message, {timeOut: 5000});
          this.router.navigate(['dashboard']);
        },
        error:(err) => {
          this.toastr.error("username or password is wrong" ,"ERROR", {timeOut: 5000});
          console.log(err);
        }
      });
    }else {
      //throw error using toastr and rquired fields
      ValidateForm.validateAllFormFields(this.loginForm);

    }
  }

}
