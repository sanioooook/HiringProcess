import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';

export interface ColumnDef {
  key: string;
  label: string;
  visible: boolean;
}

@Component({
  selector: 'app-column-selector',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatCheckboxModule, MatButtonModule],
  templateUrl: './column-selector.component.html',
  styleUrl: './column-selector.component.scss',
})
export class ColumnSelectorComponent {
  constructor(
    public dialogRef: MatDialogRef<ColumnSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { columns: ColumnDef[] },
  ) {
    // Deep-copy so Cancel doesn't mutate the original
    this.data = { columns: data.columns.map(c => ({ ...c })) };
  }

  apply(): void { this.dialogRef.close(this.data.columns); }
  cancel(): void { this.dialogRef.close(null); }
}
