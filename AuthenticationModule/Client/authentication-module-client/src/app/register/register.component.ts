import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {MatCardModule} from "@angular/material/card";
import {MatDividerModule} from "@angular/material/divider";
import {MatButtonModule} from "@angular/material/button";
import {MatProgressBarModule} from "@angular/material/progress-bar";
import {AuthenticationService} from "../services/authentication.service";

@Component({
  selector: 'au-register',
  standalone: true,
  imports: [CommonModule, MatFormFieldModule, MatInputModule, MatCardModule, MatDividerModule, MatButtonModule, MatProgressBarModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  message = "Component initial message";
  constructor(private _authenticationService: AuthenticationService) {
  }

  ngOnInit(): void {
    // this.message = this._authenticationService.tstMessage;
  }

  onClick(): void {
    this._authenticationService.test();
    this.message = this._authenticationService.tstMessage;
  }

}
