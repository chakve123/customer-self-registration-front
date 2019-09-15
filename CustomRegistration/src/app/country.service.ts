import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class CountryService {
  baseUrl = "country-url";

  constructor(private http: HttpClient) {
  }

  getCountries() {
    return this.http.get(this.baseUrl);
  }
}
