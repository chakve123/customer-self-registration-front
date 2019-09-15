import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private url = 'currencyRates';

  constructor(private http: HttpClient) { }

  getRates() {
    return this.http.get(this.url);
  }
}
