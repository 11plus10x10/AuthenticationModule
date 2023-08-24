import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {of} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  user: any = null;
  tstMessage: string = "Initial string";

  constructor(private _http: HttpClient) { }

  loadUser(): void {
    const request = this._http.get<any>("/api/user");
    request.subscribe(user => this.user = user);
  }
  signIn(): void {}
  signUp(): void {}

  test(): void {
    const request = this._http
      .get<string>("http://localhost:5190/api/user", { responseType: "text" as "json"});
    request.subscribe(m => this.tstMessage = m);
  }
}
