import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from "@angular/material/input";
import { FormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";

@Component({
  selector: 'au-password',
  standalone: true,
  imports: [CommonModule, MatInputModule, FormsModule, MatButtonModule, MatIconModule],
  templateUrl: './password.component.html',
  styleUrls: ['./password.component.scss']
})
export class PasswordComponent {
  @Input() label?: string;
  @Output() newItemEvent = new EventEmitter<string>();
  isHidden: boolean = true;

  addNewItem(value: string) {
    this.newItemEvent.emit(value);
  }
}
