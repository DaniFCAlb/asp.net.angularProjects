import { HttpClient } from '@angular/common/http';
import { Token } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from "@auth0/angular-jwt";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl: string = "https://localhost:7126/api/User/"
  private userPayload:any;


  constructor(private hhtp: HttpClient, private router: Router) {
    this.userPayload = this.decodedToken();
  }

  signUp(userObj:any){
    return this.hhtp.post<any>(`${this.baseUrl}resgister`, userObj);
  }

  login(loginObj:any){
    return this.hhtp.post<any>(`${this.baseUrl}authentitace`, loginObj)
  }

  storeToken(tokenValue: string){
    localStorage.setItem('token', tokenValue);
  }

  getToken(){
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean{
    return !!localStorage.getItem('token');
  }

  signOut(){
    localStorage.clear();
    this.router.navigate(['login']);
  }

  decodedToken() {
    const jwtHelper = new JwtHelperService();
    const token = this.getToken()!;

    console.log(jwtHelper.decodeToken(token));

    return jwtHelper.decodeToken(token);
  }

  getFullNameFromToken() {
    if(this.userPayload) {
      return this.userPayload.name;
    }
  }

  getRoleFRomToken() {
    if(this.userPayload) {
      return this.userPayload.role;
    }
  }
}
