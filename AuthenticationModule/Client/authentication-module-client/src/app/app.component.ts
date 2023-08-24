import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { RegisterComponent } from "./register/register.component";
import { RouterLink, RouterOutlet } from "@angular/router";
import { MatCardModule } from "@angular/material/card";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTabsModule } from "@angular/material/tabs";

@Component({
  selector: 'au-root',
  standalone: true,
  imports: [CommonModule, MatSlideToggleModule, MatFormFieldModule, MatInputModule, RegisterComponent, RouterOutlet, MatCardModule, MatToolbarModule, MatTabsModule, RouterLink],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'authentication-module-client';
  links = [
    { linkName: "Sign Up", routeLink: "/signup" },
    { linkName: "Sign In", routeLink: "/signin" },
  ];
  activeLink = this.links[0].linkName;
}
