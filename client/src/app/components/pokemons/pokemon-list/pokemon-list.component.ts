import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Pokemon } from 'src/app/models/Pokemon';
import { PokemonsService } from 'src/app/services/pokemons.service';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-pokemon-list',
  templateUrl: './pokemon-list.component.html',
  styleUrls: ['./pokemon-list.component.css']
})
export class PokemonListComponent implements AfterViewInit  {
  displayedColumns: string[] = ['Name', 'BirthDate', 'CategoryName'];
  dataSource: any;
  selection: any;

  constructor(private pokemonsService: PokemonsService) { }

  @ViewChild(MatPaginator) paginator: MatPaginator;

  ngAfterViewInit(): void {
    this.pokemonsService.getAllPokemons().subscribe({
      next: (pokemonPageData) => {
        
        const pokemons = Object.values(pokemonPageData)[7];
        this.dataSource = new MatTableDataSource<Pokemon>(pokemons);
        
        this.selection = new SelectionModel<Pokemon>(true, []);
      },
      error: (response) => {
        console.log(response);
      }
    });
  }

  // /** Whether the number of selected elements matches the total number of rows. */
  // isAllSelected() {
  //   const numSelected = this.selection.selected.length;
  //   const numRows = this.dataSource.data.length;
  //   return numSelected === numRows;
  // }

  // /** Selects all rows if they are not all selected; otherwise clear selection. */
  // toggleAllRows() {
  //   if (this.isAllSelected()) {
  //     this.selection.clear();
  //     return;
  //   }

  //   this.selection.select(...this.dataSource.data);
  // }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  // /** The label for the checkbox on the passed row */
  // checkboxLabel(row?: Pokemon): string {
  //   if (!row) {
  //     return `${this.isAllSelected() ? 'deselect' : 'select'} all`;
  //   }
  //   return `${this.selection.isSelected(row) ? 'deselect' : 'select'} `;
  // }

}


/**  Copyright 2023 Google LLC. All Rights Reserved.
    Use of this source code is governed by an MIT-style license that
    can be found in the LICENSE file at https://angular.io/license */