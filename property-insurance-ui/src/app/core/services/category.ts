import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private categoryUrl = 'https://localhost:7078/api/Category';
  private planUrl = 'https://localhost:7078/api/PropertyPlans';

  constructor(private http: HttpClient) { }

  getCategories() {
    return this.http.get<any[]>(this.categoryUrl);
  }

  addPlan(plan: any) {
    return this.http.post(this.planUrl, plan);
  }
}