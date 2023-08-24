import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from "@angular/material/input";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CardComponent } from "../card/card.component";
import { PasswordComponent } from "../password/password.component";
import { ISignData } from "../../interfaces/ISignData";

@Component({
  selector: 'au-signup',
  standalone: true,
  imports: [CommonModule, CardComponent, MatInputModule, ReactiveFormsModule, FormsModule, PasswordComponent],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {
  signData: ISignData = {
    email: "",
    title: "Sign Up",
    password: "",
  };
  repeatedPassword: string = "";

  onPasswordFieldInput(newValue: string): void {
    this.signData.password = newValue;
  }

  onRepeatPasswordFieldInput(newValue: string): void {
    this.repeatedPassword = newValue;
  }
}
