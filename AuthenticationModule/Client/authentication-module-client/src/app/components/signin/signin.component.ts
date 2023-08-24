import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from "@angular/material/card";
import { MatInputModule } from "@angular/material/input";
import { FormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { CardComponent } from "../card/card.component";
import { PasswordComponent } from "../password/password.component";
import { ISignData } from "../../interfaces/ISignData";

@Component({
  selector: 'au-signin',
  standalone: true,
  imports: [CommonModule, MatCardModule, CardComponent, MatInputModule, FormsModule, MatButtonModule, MatIconModule, PasswordComponent],
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css']
})
export class SigninComponent {
  signData: ISignData = {
    title: "Sign In",
    email: "",
    password: "",
  }

  onPasswordFieldInput(newValue: string) {
    this.signData.password = newValue;
  }
}
