import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { AuthService } from '../services/auth.service';
import { UserStoreService } from '../services/user-store.service';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  public users: any = [];
  public fulllName:string = "";
  public role!:string;

  constructor(private auth: AuthService, private api: ApiService, private userStore: UserStoreService){}

  ngOnInit() {
    this.api.getUsers().subscribe(
      res => {
        this.users = res;
      }
    )

    this.userStore.getFullNameFromStore().subscribe(
      res => {
        let fulllNameFromToken = this.auth.getFullNameFromToken();
        this.fulllName = fulllNameFromToken || res;
      }
    )

    this.userStore.getRoleFromStore().subscribe(val => {
      const roleFromToken = this.auth.getRoleFRomToken();
      this.role = val || roleFromToken;
    })
  }

  logout(){
    this.auth.signOut();
  }

}
