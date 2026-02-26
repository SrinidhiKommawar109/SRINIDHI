import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../../core/services/category';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category-list.html'
})
export class CategoryList implements OnInit {

  categories: any[] = [];

  selectedSubCategoryId: number | null = null;

  newPlan = {
    planName: '',
    baseCoverageAmount: 0,
    coverageRate: 0,
    subCategoryId: 0
  };

  constructor(private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (res) => {
        this.categories = res;
      },
      error: (err) => {
        console.error('Error loading categories', err);
      }
    });
  }

  savePlan() {

    if (!this.selectedSubCategoryId) {
      alert('Please select sub category');
      return;
    }

    this.newPlan.subCategoryId = this.selectedSubCategoryId;

    this.categoryService.addPlan(this.newPlan).subscribe({
      next: () => {
        alert('Plan Added Successfully ✅');
        this.resetForm();
        this.loadCategories();
      },
      error: (err) => {
        console.error('Error adding plan', err);
        alert('Error adding plan');
      }
    });
  }

  resetForm() {
    this.newPlan = {
      planName: '',
      baseCoverageAmount: 0,
      coverageRate: 0,
      subCategoryId: 0
    };
    this.selectedSubCategoryId = null;
  }
}